using System;

namespace Raptor.Game.Shared
{
    public static class Configuration
    {
        public const int ServerPort = 12345;
        public const int TickRate = 32;
        public static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1.0 / TickRate);
        public static readonly TimeSpan CommandBuffer = TimeSpan.FromMilliseconds(64);
        public static readonly TimeSpan InterpolationWindow = TimeSpan.FromMilliseconds(128);
        public static readonly float PlayerSpeed = 8;
    }
}