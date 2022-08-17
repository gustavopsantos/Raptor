using System;

namespace Raptor.Game.Shared.Timing
{
    [Serializable]
    public readonly struct TimingInfo
    {
        public DateTime TimerStartedAt { get; }

        public TimingInfo(DateTime timerStartedAt)
        {
            TimerStartedAt = timerStartedAt;
        }
    }
}