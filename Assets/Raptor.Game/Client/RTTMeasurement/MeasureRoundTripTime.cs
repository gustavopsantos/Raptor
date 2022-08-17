using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raptor.Game.Shared.RTTMeasurement;

namespace Raptor.Game.Client.RTTMeasurement
{
    public static class MeasureRoundTripTime
    {
        public static async Task<TimeSpan> Measure(IPEndPoint host, RaptorClient mean)
        {
            var timeAtRequest = DateTime.Now;
            await mean.Request<Ping, Pong>(new Ping(), host, CancellationToken.None).ConfigureAwait(false);
            var timeAtResponse = DateTime.Now;
            return timeAtResponse - timeAtRequest;
        }
    }
}