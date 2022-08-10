using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct PlayerInfo
    {
        public Guid PlayerId { get; }

        public PlayerInfo(Guid playerId)
        {
            PlayerId = playerId;
        }
    }
}