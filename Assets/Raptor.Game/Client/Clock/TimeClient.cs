using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raptor.Game.Shared.Clock;
using Raptor.Game.Client.RTTMeasurement;

namespace Raptor.Game.Client.Clock
{
    public class TimeClient
    {
        private DateTime _localTimeAtLastSync;
        private DateTime _serverTimeAtLastSync;

        public DateTime Time => _serverTimeAtLastSync + ElapsedTimeSinceLastSync;
        private TimeSpan ElapsedTimeSinceLastSync => LocalTime() - _localTimeAtLastSync;

        private static DateTime LocalTime()
        {
            return DateTime.Now;
        }

        public static async Task<TimeClient> Synchronized(IPEndPoint server, RaptorClient mean)
        {
            var timeClient = new TimeClient();
            await timeClient.Sync(server, mean);
            return timeClient;
        }

        private async Task Sync(IPEndPoint server, RaptorClient mean)
        {
            // At the moment time server arrives client, it is already outdated by half rtt
            // We want client to run half rtt ahead of server
            // so client commands arrive just in time to be processed on server

            var roundTripTime = await MeasureMedianRoundTripTime.Measure(server, mean);
            var response = await mean.Request<GetTime, DateTime>(new GetTime(), server, CancellationToken.None);
            _localTimeAtLastSync = LocalTime();
            _serverTimeAtLastSync = response.Payload.Add(roundTripTime / 2);
        }
    }
}