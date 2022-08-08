using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Raptor.Interface
{
    public class Sequence<T>
    {
        public Guid Id { get; }
        public Guid Replies { get; }
        public T Payload { get; }
        public IPEndPoint Source { get; }
        public RaptorClient Mean { get; }

        public Sequence(Guid id, Guid replies, T payload, IPEndPoint source, RaptorClient mean)
        {
            Id = id;
            Replies = replies;
            Payload = payload;
            Source = source;
            Mean = mean;
        }

        public Task Reply<TReply>(TReply payload, CancellationToken cancellationToken)
        {
            return Mean.Reply(Id, payload, Source, cancellationToken);
        }
        
        public Task<Sequence<TAwait>> ReplyAndAwait<TReply, TAwait>(TReply payload, CancellationToken cancellationToken)
        {
            return Mean.ReplySequenceAndAwait<TReply, TAwait>(Id, payload, Source, cancellationToken);
        }
    }
}