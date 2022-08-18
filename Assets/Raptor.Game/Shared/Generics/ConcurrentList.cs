using System;
using System.Collections;
using System.Collections.Generic;

namespace Raptor.Game.Shared.Generics
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly object _lock;
        private readonly List<T> _registry = new();

        public ConcurrentList()
        {
            _lock = ((ICollection) _registry).SyncRoot;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                _registry.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _registry.Remove(item);
            }
        }

        public void RemoveAll(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                for (var i = 0; i < _registry.Count; i++)
                {
                    if (predicate.Invoke(_registry[i]))
                    {
                        _registry.RemoveAt(i);
                    }
                }
            }
        }
        
        public List<T> Clone()
        {
            lock (_lock)
            {
                return new List<T>(_registry);
            }
        }
    }
}