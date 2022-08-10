using System;

namespace Raptor.Game.Shared
{
    public static class DateTimeExtensions
    {
        public static string Print(this DateTime dateTime)
        {
            return $"{dateTime.Second}:{dateTime.Millisecond}";
        }
    }
}