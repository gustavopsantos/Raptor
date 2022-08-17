using System;
using System.Net;
using Raptor.Enums;
using UnityEngine.Assertions;
using System.Collections.Concurrent;
using System.Linq;

namespace Raptor.ValueObjects
{
    internal class PacketSequenceStorage
    {
        internal class Route
        {
            private readonly ConcurrentDictionary<(IPEndPoint, Acquisition), int> _registry = new();

            internal void Initialize(IPEndPoint endpoint)
            {
                foreach (var acquisition in Enum.GetValues(typeof(Acquisition)).Cast<Acquisition>())
                {
                    Set(endpoint, acquisition, 0);
                }
            }
            
            internal void Remove(IPEndPoint endpoint)
            {
                foreach (var acquisition in Enum.GetValues(typeof(Acquisition)).Cast<Acquisition>())
                {
                    _registry.TryRemove((endpoint, acquisition), out _);
                }
            }
            
            internal int Get(IPEndPoint endpoint, Acquisition acquisition)
            {
                Assert.IsTrue(_registry.ContainsKey((endpoint, acquisition)), "Was host initialized during connection?");
                return _registry[(endpoint, acquisition)];
            }

            internal int Increment(IPEndPoint endpoint, Acquisition acquisition)
            {
                var value = Get(endpoint, acquisition) + 1;
                Set(endpoint, acquisition, value);
                return value;
            }

            internal void Set(IPEndPoint endpoint, Acquisition acquisition, int value)
            {
                _registry[(endpoint, acquisition)] = value;
            }

            internal void Clear()
            {
                _registry.Clear();
            }
        }

        public readonly Route Incoming = new();
        public readonly Route Outgoing = new();

        public PacketSequenceStorage(IPEndPoint endpoint)
        {
            Incoming.Initialize(endpoint);
            Outgoing.Initialize(endpoint);
        }

        public void Remove(IPEndPoint endpoint)
        {
            Incoming.Remove(endpoint);
            Outgoing.Remove(endpoint);
        }
    }
}