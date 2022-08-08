using System;
using System.Collections.Generic;
using System.Net;
using Raptor.Connection;
using Raptor.Enums;
using Raptor.Extensions;
using Raptor.Packets;
using Raptor.ValueObjects;
using UnityEngine;

namespace Raptor
{
    internal class ShouldAcquirePacket
    {
        private readonly PacketSequenceStorage _storage;
        private readonly IReadOnlyDictionary<IPEndPoint, ConnectionState> _connections;

        internal ShouldAcquirePacket(
            PacketSequenceStorage storage,
            IReadOnlyDictionary<IPEndPoint, ConnectionState> connections
        )
        {
            _storage = storage;
            _connections = connections;
        }

        internal bool Check(Packet packet, IPEndPoint source)
        {
            switch (packet.Acquisition)
            {
                case Acquisition.Always: return true;
                case Acquisition.Ordered: return HasConnection(packet, source) && IsOrdered(packet, source);
                case Acquisition.Sequenced: return HasConnection(packet, source) && IsSequenced(packet, source);
                default: throw new NotImplementedException(packet.Acquisition.ToString());
            }
        }

        private bool HasConnection(Packet packet, IPEndPoint source)
        {
            var hasConnection = _connections.TryGetValue(source, out _);

            if (!hasConnection)
            {
                Debug.LogWarning($"Discarding payload {packet.Payload.GetType().ReadableName()}" +
                                 $" from {source} because theres no such connection");
            }

            return hasConnection;
        }

        private bool IsOrdered(Packet packet, IPEndPoint source)
        {
            var lastAcquiredSequence = _storage.Incoming.Get(source, packet.Acquisition);
            var isOrdered = packet.Sequence > lastAcquiredSequence;

            if (!isOrdered)
            {
                Debug.LogWarning($"Discarding payload {packet.Payload.GetType().ReadableName()} {packet.Sequence}" +
                                 $" from {source} because it is not ordered, last packet was {lastAcquiredSequence}");
            }

            return isOrdered;
        }

        private bool IsSequenced(Packet packet, IPEndPoint source)
        {
            var lastAcquiredSequence = _storage.Incoming.Get(source, packet.Acquisition);
            var isSequenced = packet.Sequence == lastAcquiredSequence + 1;

            if (!isSequenced)
            {
                Debug.LogWarning($"Discarding payload {packet.Payload.GetType().ReadableName()} {packet.Sequence}" +
                                 $" from {source} because it is not sequenced, last packet was {lastAcquiredSequence}");
            }

            return isSequenced;
        }
    }
}