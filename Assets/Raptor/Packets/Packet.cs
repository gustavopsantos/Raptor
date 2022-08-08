using System;
using Raptor.Enums;

namespace Raptor.Packets
{
    [Serializable]
    public class Packet
    {
        public int Sequence { get; }
        public Delivery Delivery { get; }
        public Acquisition Acquisition { get; }
        public object Payload { get; }

        public Packet(int sequence, Delivery delivery, Acquisition acquisition, object payload)
        {
            Sequence = sequence;
            Delivery = delivery;
            Acquisition = acquisition;
            Payload = payload;
        }
    }
}