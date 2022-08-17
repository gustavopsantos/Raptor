using System;

namespace Raptor.Game.Shared
{
    [Serializable]
    public readonly struct Ticked<T>
    {
        public int Tick { get; }
        public T Value { get; }

        public Ticked(int tick, T value)
        {
            Tick = tick;
            Value = value;
        }
    }
}