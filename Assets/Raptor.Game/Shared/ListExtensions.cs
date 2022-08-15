using System;
using System.Collections.Generic;

namespace Raptor.Game.Shared
{
    public static class ListExtensions
    {
        public static bool TryGet<T>(this List<T> list, Func<T, bool> predicate, out T item)
        {
            foreach (var i in list)
            {
                if (predicate.Invoke(i))
                {
                    item = i;
                    return true;
                }
            }

            item = default;
            return false;
        }
    }
}