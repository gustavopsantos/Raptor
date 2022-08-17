using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct Snapshot
    {
        public Ticked<(float, float)> Position { get; }

        public Snapshot(Ticked<(float, float)> position)
        {
            Position = position;
        }
    }
}