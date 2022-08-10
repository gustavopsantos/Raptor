using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Raptor.Game.Client.NetworkStatistics
{
    public static class MeasureMedianRoundTripTime
    {
        public static async Task<TimeSpan> Measure(IPEndPoint host, RaptorClient mean)
        {
            var samples = new TimeSpan[5];
            
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = await MeasureRoundTripTime.Measure(host, mean);
            }
            
            Array.Sort(samples);
            
            Debug.Log($"Measured RTTs {string.Join(", ", samples.Select(s => $"{s.TotalMilliseconds}ms"))}");
            return samples[samples.Length / 2];
        }
    }
}
