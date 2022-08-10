using System;

namespace Raptor.Game.Shared
{
    public static class Configuration
    {
        public const int ServerPort = 12345;
        public const int TickRate = 2;
        public static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1.0 / TickRate);
    }
}