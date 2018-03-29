using System;
using System.Linq;
using Roham.Lib.Caches;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Roham.Caching
{
    public class MemCache : Cache<MemoryCache>
    {
        protected MemoryCache _cache;

        public MemCache() : base()
        {
            _cache = MemoryCache.Default;
        }

        public MemCache(TimeSpan cacheDuration) : base(cacheDuration)
        {
            _cache = MemoryCache.Default;
        }

        public override bool Exists(string key)
        {
            return _cache.Get(key) != null;
        }

        public override bool TryGet<T>(string key, out T @object) 
        {
            return TryGetInternal(key, out @object);
        }

        public override void Set<T>(string key, T @object, TimeSpan? slidingExpiration = null)
        {   
            if (@object == null)
            {
                return;
            }
            var cacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration ?? CacheDuration
            };            
            _cache.Set(key, @object, cacheItemPolicy);
        }

        public override void Set<T>(string key, T @object, DateTime absoluteExpiration)
        {
            if (@object == null)
            {
                return;
            }
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
            };
            _cache.Set(key, @object, cacheItemPolicy);
        }

        public override bool TryGetHashSet<T>(string key, out HashSet<T> hashSet)
        {
            return TryGetInternal(key, out hashSet);
        }

        public override void SetHashSet<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null)
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration ?? CacheDuration,
            };
            _cache.Set(key, hashSet, cacheItemPolicy);
        }

        public override void SetHashSet<T>(string key, HashSet<T> HashSet, DateTime absoluteExpiration)
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration
            };
            _cache.Set(key, HashSet, cacheItemPolicy);
        }

        public override void Remove(string key)
        {
            _cache.Remove(key);
        }

        public override void ClearAll()
        {
            var allKeys = _cache.Select(o => o.Key);
            Parallel.ForEach(allKeys, key => _cache.Remove(key));
        }

        private bool TryGetInternal<T>(string key, out T value)
        {
            try
            {
                if (_cache[key] == null)
                {
                    value = default(T);
                    return false;
                }
                value = (T)_cache[key];
            }
            catch
            {
                value = default(T);
                return false;
            }
            return true;
        }
    }
}
