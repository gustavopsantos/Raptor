using System;
using System.Net;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Raptor.Game.Client.RTTMeasurement
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
            return Median(samples);
        }

        private static TimeSpan Median(IReadOnlyList<TimeSpan> samples)
        {
            return samples[samples.Count / 2];
        }
    }
}