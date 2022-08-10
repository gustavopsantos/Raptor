using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct GetServerTime
    {
        public DateTime Time { get; }

        public GetServerTime(DateTime time)
        {
            Time = time;
        }
    }
}