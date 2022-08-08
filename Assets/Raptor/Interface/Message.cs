using System.Net;

namespace Raptor.Interface
{
    public class Message<T>
    {
        public T Payload { get; }
        public IPEndPoint Source { get; }

        public Message(T payload, IPEndPoint source)
        {
            Payload = payload;
            Source = source;
        }
    }
}