using System.Threading;
using Raptor.Interface;
using Raptor.Game.Shared.RTTMeasurement;

namespace Raptor.Game.Server.RTTMeasurement
{
    public class PingRequestHandler : IRequestHandler<Ping>
    {
        public void Handle(Sequence<Ping> request)
        {
            request.Reply(new Pong(), CancellationToken.None);
        }
    }
}