using System;

namespace Raptor.Packets
{
    [Serializable]
    public class Ack
    {
        public int Sequence { get; }

        public Ack(int sequence)
        {
            Sequence = sequence;
        }
    }
}