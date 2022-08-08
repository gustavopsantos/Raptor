using System;

namespace Raptor
{
    public static class TimeProfiler
    {
        private static DateTime _startedAt;
        
        public static void Start()
        {
            _startedAt = DateTime.Now;
        }
        
        public static string Sample()
        {
            return (DateTime.Now - _startedAt).TotalMilliseconds.ToString("F1");
        }
    }
}
