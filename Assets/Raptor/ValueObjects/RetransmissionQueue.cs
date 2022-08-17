using System;
using System.Collections.Concurrent;
using System.Net;
using System.Linq;
using UnityEngine;
using Raptor.Settings;
using System.Threading;
using Raptor.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raptor.Enums;
using Raptor.Packets;
using UnityEngine.Assertions;

namespace Raptor.ValueObjects
{
    public class RetransmissionQueue : IDisposable
    {
        private readonly RaptorClient _mean;
        private readonly Thread _retransmissionThread;
        private readonly ConcurrentDictionary<IPEndPoint, List<Packet>> _pending = new();
        private readonly ConcurrentDictionary<(int, Acquisition, IPEndPoint), TaskCompletionSource<object>> _awaiters = new();

        public RetransmissionQueue(RaptorClient mean)
        {
            _mean = mean;
            _retransmissionThread = new Thread(RetransmitReliableMessages);
            _retransmissionThread.Start();

            _mean.RegisterObjectHandler<Ack>((ack, source) =>
            {
                //Debug.Log($"Acking sequence {ack.Sequence} from {source} at {TimeProfiler.Sample()}  thread: {AppDomain.GetCurrentThreadId()}");
                
                lock (this)
                {
                    if (_awaiters.TryRemove((ack.Sequence, ack.Acquisition, source), out var awaiter))
                    {
                        Assert.IsNotNull(awaiter);
                        awaiter.TrySetResult(null);
                        _pending[source].RemoveAll(p => p.Sequence == ack.Sequence);
                    }
                }

                //Debug.Log($"Acked sequence {ack.Sequence} from {source} at {TimeProfiler.Sample()}  thread: {AppDomain.GetCurrentThreadId()}");
            });
        }

        public void Dispose()
        {
            _pending.Clear();
            _awaiters.Clear();
            _retransmissionThread.Abort();
        }

        private void RetransmitReliableMessages()
        {
            while (true)
            {
                Thread.Sleep(Configuration.RetransmissionInterval);

                lock (this)
                {
                    foreach (var (address, packets) in _pending.Select(kvp => (kvp.Key, kvp.Value)))
                    {
                        foreach (var packet in packets.Take(Configuration.RetransmissionCapacity))
                        {
                            _mean.SendPacket(packet, address);
                        }
                    }
                }
            }
        }

        public TaskCompletionSource<object> Add(Packet packet, IPEndPoint recipient)
        {
            Debug.LogWarning($"Adding msg with seq {packet.Sequence} {packet.Delivery} {packet.Acquisition} {recipient} {packet.Payload.GetType().ReadableName()}");

            lock (this)
            {
                if (!_pending.ContainsKey(recipient))
                {
                    _pending.TryAdd(recipient, new List<Packet>());
                }

                var queue = _pending[recipient];

                if (queue.Count(msg => msg.Sequence == packet.Sequence && msg.Acquisition == packet.Acquisition) != 0)
                {
                    throw new Exception(
                        $"Something went wrong! Tried to add msg with seq {packet.Sequence} {packet.Delivery} {packet.Acquisition} {recipient} {packet.Payload.GetType().ReadableName()}");
                }

                _pending[recipient].Add(packet);
                var tcs = new TaskCompletionSource<object>();
                tcs.Task.ConfigureAwait(false);
                _awaiters.TryAdd((packet.Sequence, packet.Acquisition, recipient), tcs);
                return tcs;
            }
        }

        public void Remove(Packet packet, IPEndPoint recipient)
        {
            lock (this)
            {
                _pending[recipient].Remove(packet);

                if (_awaiters.TryGetValue((packet.Sequence, packet.Acquisition, recipient), out var awaiter))
                {
                    awaiter.TrySetCanceled();
                }
            }
        }
    }
}