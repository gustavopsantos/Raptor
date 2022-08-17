using System.Threading;
using Raptor.Interface;
using Raptor.Game.Shared.Timing;
using Timer = Raptor.Game.Shared.Timing.Timer;

namespace Raptor.Game.Server.Timing
{
    public class TimingServer : IRequestHandler<GetTimingInfo>
    {
        private readonly Timer _serverTimer;

        public TimingServer(Timer serverTimer)
        {
            _serverTimer = serverTimer;
        }

        public void Handle(Sequence<GetTimingInfo> request)
        {
            request.Reply(new TimingInfo(_serverTimer.StartedAt), CancellationToken.None);
        }
    }
}