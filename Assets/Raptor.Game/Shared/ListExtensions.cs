using System;
using System.Collections.Generic;
using System.Linq;

namespace Raptor.Game.Shared
{
    public static class ListExtensions
    {
        public static bool TryGet<T>(this IEnumerable<T> list, Func<T, bool> predicate, out T item)
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
        
        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
        {
            foreach (var i in list.Reverse())
            {
                if (predicate.Invoke(i))
                {
                    list.Remove(i);
                }
            }
        }
    }
}