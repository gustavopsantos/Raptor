using System;

namespace Raptor.Settings
{
    internal static class Configuration
    {
        internal const int RetransmissionCapacity = 64;
        internal static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(2);
        internal static readonly TimeSpan RetransmissionInterval = TimeSpan.FromMilliseconds(32);
    }
}