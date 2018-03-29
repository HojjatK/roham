using System;
using System.Collections.Concurrent;

namespace Roham.Lib.Collections
{
    public class ObjectLookup
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _cacheMap =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        public T GetOrAdd<T>(string key, Func<T> initFunc)
        {
            var entry = _cacheMap.GetOrAdd(typeof(T), new ConcurrentDictionary<string, object>());
            return (T)entry.GetOrAdd(key, initFunc());
        }
    }
}
