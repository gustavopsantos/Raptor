using System;

namespace Raptor.Packets
{
    [Serializable]
    public class NetMessage<T>
    {
        public T Payload;

        public NetMessage(T payload)
        {
            Payload = payload;
        }
    }
}