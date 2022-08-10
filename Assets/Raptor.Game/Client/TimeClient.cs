using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raptor.Game.Shared;

namespace Raptor.Game.Client
{
    public class TimeClient
    {
        private readonly IPEndPoint _host;
        private readonly RaptorClient _mean;
        public DateTime Time => _clientTime.Add(ClientMachineTimeNow() - _timeAtResponse);

        private DateTime _clientTime;
        private DateTime _timeAtResponse;

        public TimeClient(IPEndPoint host, RaptorClient mean)
        {
            _host = host;
            _mean = mean;
        }
        
        public async Task Sync(TimeSpan roundTripTime)
        {
            // At the moment time server arrives client, it is already outdated by half rtt
            // We want client to run half rtt ahead of server
            // so client commands arrive just in time to be processed on server

            var timePacket = await _mean
                .Request<GetServerTime, DateTime>(new GetServerTime(), _host, CancellationToken.None);
            
            _clientTime = timePacket.Payload.Add(roundTripTime/2);
            _timeAtResponse = ClientMachineTimeNow();
        }

        private static DateTime ClientMachineTimeNow()
        {
            return DateTime.Now;
        }
    }
}
