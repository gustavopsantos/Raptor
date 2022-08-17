using System;

namespace Raptor.Game.Shared.Clock
{
    [Serializable]
    public readonly struct GetTime
    {
        public DateTime Time { get; }

        public GetTime(DateTime time)
        {
            Time = time;
        }
    }
}