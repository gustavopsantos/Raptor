using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raptor.Game.Shared.Clock;

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

        public static async Task<TimeClient> Synchronized(IPEndPoint server, RaptorClient mean, TimeSpan rtt)
        {
            var timeClient = new TimeClient();
            await timeClient.Sync(server, mean, rtt);
            return timeClient;
        }

        private async Task Sync(IPEndPoint server, RaptorClient mean, TimeSpan rtt)
        {
            // At the moment time server arrives client, it is already outdated by half rtt
            // We want client to run half rtt ahead of server
            // so client commands arrive just in time to be processed on server

            var response = await mean.Request<GetTime, DateTime>(new GetTime(), server, CancellationToken.None);
            _localTimeAtLastSync = LocalTime();
            _serverTimeAtLastSync = response.Payload.Add(rtt / 2);
        }
    }
}