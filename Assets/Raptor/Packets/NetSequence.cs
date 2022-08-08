using System;

namespace Raptor.Packets
{
    [Serializable]
    public class NetSequence<T>
    {
        public Guid Id { get; }
        public Guid Replies { get; }
        public T Payload { get; }

        public NetSequence(Guid id, Guid replies, T payload)
        {
            Id = id;
            Replies = replies;
            Payload = payload;
        }
    }
}