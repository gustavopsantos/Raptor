using System;

namespace Raptor.Game.Shared.GameInput
{
    [Serializable]
    public struct Input
    {
        public sbyte Horizontal;
        public sbyte Vertical;

        public void Clear()
        {
            Horizontal = 0;
            Vertical = 0;
        }
    }
}