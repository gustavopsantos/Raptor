using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct PlayerCommand
    {
        public int Tick { get; }
        public sbyte Horizontal { get; }
        public sbyte Vertical { get; }

        public PlayerCommand(int tick, sbyte horizontal, sbyte vertical)
        {
            Tick = tick;
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }
}