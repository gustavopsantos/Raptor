using System;
using Raptor.Enums;

namespace Raptor.Packets
{
    [Serializable]
    public class Ack
    {
        public int Sequence { get; }
        public Acquisition Acquisition { get; }

        public Ack(int sequence, Acquisition acquisition)
        {
            Sequence = sequence;
            Acquisition = acquisition;
        }
    }
}