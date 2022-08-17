using System;
using System.Threading;
using Raptor.Interface;
using Raptor.Game.Shared.Clock;

namespace Raptor.Game.Server.Clock
{
    public class TimeServer : IRequestHandler<GetTime>
    {
        public void Handle(Sequence<GetTime> request)
        {
            request.Reply(DateTime.UtcNow, CancellationToken.None);
        }
    }
}