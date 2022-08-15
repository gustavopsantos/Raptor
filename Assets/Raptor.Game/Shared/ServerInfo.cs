using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct ServerInfo
    {
        public DateTime TimerStartedAt { get; }

        public ServerInfo(DateTime timerStartedAt)
        {
            TimerStartedAt = timerStartedAt;
        }
    }
}