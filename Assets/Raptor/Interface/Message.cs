using System.Net;

namespace Raptor.Interface
{
    public class Message<T>
    {
        public T Payload { get; }
        public IPEndPoint Source { get; }
        public RaptorClient Mean { get; }

        public Message(T payload, IPEndPoint source, RaptorClient mean)
        {
            Payload = payload;
            Source = source;
            Mean = mean;
        }
    }
}