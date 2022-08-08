using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using Raptor.Enums;
using Raptor.Packets;
using Raptor.Interface;
using Raptor.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Raptor.Connection;
using Raptor.Extensions;
using Raptor.Settings;
using Raptor.ValueObjects;
using UnityEngine;
using UnityEngine.Assertions;

namespace Raptor
{
    public class RaptorClient : IDisposable
    {
        private readonly DatagramClient _datagramClient;
        private readonly ShouldAcquirePacket _shouldAcquirePacket;
        private readonly RetransmissionQueue _retransmissionQueue;
        private readonly PacketSequenceStorage _sequenceHistory = new();
        private readonly ConcurrentDictionary<IPEndPoint, ConnectionState> _connections = new();
        private readonly ConcurrentDictionary<Type, Action<object, IPEndPoint>> _handlers = new();
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _awaiters = new();

        public RaptorClient(int port)
        {
            _retransmissionQueue = new RetransmissionQueue(this);
            _datagramClient = new DatagramClient(HandleDatagram, port);
            _shouldAcquirePacket = new ShouldAcquirePacket(_sequenceHistory, _connections);
            RegisterRequestHandler<ConnectionRequest>(HandleConnectionRequestAsync);
        }

        public void Dispose()
        {
            _retransmissionQueue.Dispose();
            _datagramClient.Dispose();
        }

        internal void SendPacket(Packet packet, IPEndPoint recipient)
        {
            var bytes = BinarySerializer.Serialize(packet);
            _datagramClient.SendDatagram(bytes, recipient);
        }

        private Packet SendPayload(object payload, IPEndPoint recipient, Delivery delivery, Acquisition acquisition)
        {
            var sequence = delivery == Delivery.Unreliable && acquisition == Acquisition.Always
                ? -1
                : _sequenceHistory.Outgoing.Increment(recipient, acquisition);
            
            var packet = new Packet(sequence, delivery, acquisition, payload);
            SendPacket(packet, recipient);
            return packet;
        }

        private void SendPacketUnreliable(object payload, IPEndPoint recipient)
        {
            Assert.IsTrue(_connections.ContainsKey(recipient), 
                "Cant send unreliable messages to a endpoint that is not connected");
            
            SendPayload(payload, recipient, Delivery.Unreliable, Acquisition.Ordered);
        }

        private Task SendPacketReliable(object payload, IPEndPoint recipient, Acquisition acquisition, CancellationToken cancellationToken)
        {
            var packet = SendPayload(payload, recipient, Delivery.Reliable, acquisition);
            var awaiter = _retransmissionQueue.Add(packet, recipient);

            cancellationToken.Register(() =>
            {
                _retransmissionQueue.Remove(recipient, packet.Sequence);
                awaiter.TrySetCanceled();
            });

            return awaiter.Task;
        }

        private void HandleDatagram(byte[] bytes, IPEndPoint source)
        {
            var packet = (Packet) BinarySerializer.Deserialize(bytes);
            
            //Debug.Log($"Packet {packet.Payload.GetType().ReadableName()} received from {source} at {TimeProfiler.Sample()}");

            if (!_shouldAcquirePacket.Check(packet, source))
            {
                return;
            }
            
            AcquirePacket(packet, source);
            AckIfRequired(packet, source);
            HandlePayload(packet, source);
        }

        private void AcquirePacket(Packet packet, IPEndPoint source)
        {
            _sequenceHistory.Incoming.Set(source, packet.Acquisition, packet.Sequence);
            Debug.Log($"Packet {packet.Payload.GetType().ReadableName()} acquired from {source} at {TimeProfiler.Sample()}");
        }

        private void AckIfRequired(Packet packet, IPEndPoint source)
        {
            if (packet.Delivery == Delivery.Reliable)
            {
                SendPayload(new Ack(packet.Sequence), source, Delivery.Unreliable, Acquisition.Always);
            }
        }

        private void HandlePayload(Packet packet, IPEndPoint source)
        {
            //Debug.Log($"Handling {packet.Payload.GetType().ReadableName()} from {source} at {TimeProfiler.Sample()} thread: {AppDomain.GetCurrentThreadId()}");
            if (_handlers.TryGetValue(packet.Payload.GetType(), out var handler))
            {
                handler.Invoke(packet.Payload, source);
            }
            //Debug.Log($"Handled {packet.Payload.GetType().ReadableName()} from {source} at {TimeProfiler.Sample()} thread: {AppDomain.GetCurrentThreadId()}");
        }

        public async Task ConnectAsync(IPEndPoint host)
        {
            _sequenceHistory.Initialize(host);
            _connections.TryAdd(host, ConnectionState.Connecting);
            
            try
            {
                using var timeout = new CancellationTokenSource(Configuration.ConnectionTimeout);
                
                var connectionResponse = await SendSequence<ConnectionRequest, ConnectionResponse>(Guid.Empty, new ConnectionRequest(), host, Acquisition.Always, timeout.Token);
                var handshakeResponse = await connectionResponse.ReplyAndAwait<HandshakeRequest, HandshakeResponse>(new HandshakeRequest(), timeout.Token);
                _connections.TryUpdate(host, ConnectionState.Connected, ConnectionState.Connecting);
            }
            catch (Exception e)
            {
                _connections.TryRemove(host, out _);
                _sequenceHistory.Remove(host);
                Debug.LogError(e);
                throw;
            }
        }
        
        private async void HandleConnectionRequestAsync(Sequence<ConnectionRequest> connectionRequest)
        {
            _sequenceHistory.Initialize(connectionRequest.Source);
            _connections.TryAdd(connectionRequest.Source, ConnectionState.Connecting);
            
            try
            {
                using var timeout = new CancellationTokenSource(Configuration.ConnectionTimeout);
                var handshakeRequest = await connectionRequest.ReplyAndAwait<ConnectionResponse, HandshakeRequest>(new ConnectionResponse(), timeout.Token);
                _connections.TryUpdate(connectionRequest.Source, ConnectionState.Connected, ConnectionState.Connecting);
                await handshakeRequest.Reply(new HandshakeResponse(), timeout.Token);
            }
            catch (Exception e)
            {
                _connections.TryRemove(connectionRequest.Source, out _);
                _sequenceHistory.Remove(connectionRequest.Source);
                Debug.LogError(e);
                throw;
            }
        }

        public void SendMessageUnreliable<T>(T payload, IPEndPoint recipient)
        {
            var netMessage = new NetMessage<T>(payload);
            SendPacketUnreliable(netMessage, recipient);
        }

        public Task SendMessageReliable<T>(T payload, IPEndPoint recipient, CancellationToken cancellationToken)
        {
            var netMessage = new NetMessage<T>(payload);
            return SendPacketReliable(netMessage, recipient, Acquisition.Sequenced, cancellationToken);
        }

        public Task BroadcastMessageReliable<T>(T payload, CancellationToken cancellationToken)
        {
            var netMessage = new NetMessage<T>(payload);
            var connections = _connections.Where(c => c.Value == ConnectionState.Connected).Select(kvp => kvp.Key);
            var tasks = connections.Select(c => SendPacketReliable(netMessage, c, Acquisition.Sequenced, cancellationToken));
            return Task.WhenAll(tasks);
        }

        private async Task<Sequence<TResponse>> SendSequence<TRequest, TResponse>(Guid replies, TRequest payload, IPEndPoint recipient, Acquisition acquisition, CancellationToken cancellationToken)
        {
            if (!_handlers.ContainsKey(typeof(NetSequence<TResponse>)))
            {
                _handlers.TryAdd(typeof(NetSequence<TResponse>), (obj, sender) =>
                {
                    var netSequence = (NetSequence<TResponse>) obj;
                    var sequence = new Sequence<TResponse>(
                        netSequence.Id, netSequence.Replies, netSequence.Payload, sender, this);

                    var awaiter = _awaiters[netSequence.Replies];
                    awaiter.TrySetResult(sequence);
                });
            }

            var netSequence = new NetSequence<TRequest>(Guid.NewGuid(), replies, payload);
            
            var tcs = new TaskCompletionSource<object>();
            _awaiters.TryAdd(netSequence.Id, tcs);
            
            await SendPacketReliable(netSequence, recipient, acquisition, cancellationToken).ConfigureAwait(false);
            var result = (Sequence<TResponse>) await tcs.Task;
            return result;
        }

        public Task<Sequence<TResponse>> Request<TRequest, TResponse>(TRequest payload, IPEndPoint recipient, CancellationToken cancellationToken)
        {
            return SendSequence<TRequest, TResponse>(Guid.Empty, payload, recipient, Acquisition.Sequenced, cancellationToken);
        }
        
        internal Task Reply<TQuery>(Guid replies, TQuery payload, IPEndPoint recipient, CancellationToken cancellationToken)
        {
            var netSequence = new NetSequence<TQuery>(Guid.NewGuid(), replies, payload);
            return SendPacketReliable(netSequence, recipient, Acquisition.Sequenced, cancellationToken);
        }
                
        internal Task<Sequence<TResponse>> ReplySequenceAndAwait<TRequest, TResponse>(Guid replies, TRequest payload, IPEndPoint recipient, CancellationToken cancellationToken)
        {
            return SendSequence<TRequest, TResponse>(replies, payload, recipient, Acquisition.Sequenced, cancellationToken);
        }

        internal void RegisterObjectHandler<T>(Action<T, IPEndPoint> handler)
        {
            _handlers.TryAdd(typeof(T), (obj, sender) => { handler.Invoke((T) obj, sender); });
        }

        public void RegisterMessageHandler<T>(Action<Message<T>> handler)
        {
            _handlers.TryAdd(typeof(NetMessage<T>), (obj, sender) =>
            {
                var netMessage = (NetMessage<T>) obj;
                var message = new Message<T>(netMessage.Payload, sender);
                handler.Invoke(message);
            });
        }
        
        public void RegisterRequestHandler<T>(Action<Sequence<T>> handler)
        {
            _handlers.TryAdd(typeof(NetSequence<T>), (obj, sender) =>
            {
                var netSequence = (NetSequence<T>) obj;
                var sequence = new Sequence<T>(netSequence.Id, netSequence.Replies, netSequence.Payload, sender, this);
                var isEntryPoint = netSequence.Replies == Guid.Empty;

                if (isEntryPoint)
                {
                    handler.Invoke(sequence);
                }
                else if (_awaiters.TryGetValue(netSequence.Replies, out var awaiter))
                {
                    awaiter.TrySetResult(sequence);
                }
            });
        }
    }
}