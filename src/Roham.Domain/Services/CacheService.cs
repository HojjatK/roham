using System;
using System.Linq;
using Roham.Data;
using Roham.Domain.Configs;
using Roham.Lib.Caches;
using System.Threading;
using System.Collections.Generic;
using Roham.Lib.Ioc;
using System.Threading.Tasks;
using Roham.Lib.Logger;
using Roham.Lib.Domain.Cache;

namespace Roham.Domain.Services
{
    public interface ICacheService
    {
        ICache MemoryCache { get; }
        int CachedSetsCount { get; }

        T Get<T>(CacheKey key, Func<T> loadAction) where T : ICacheable;
        T Get<T>(CacheIndex index, Func<T> loadAction) where T : ICacheable;
        void Set<T>(T @object, TimeSpan? slidingExpiration = null) where T : ICacheable;
        void Set<T>(T @object, DateTime absoluteExpiration) where T : ICacheable;

        HashSet<T> GetHash<T>(string key, Func<HashSet<T>> loadAction) where T : ICacheable;
        void SetHash<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null) where T : ICacheable;
        void SetHash<T>(string key, HashSet<T> hashSet, DateTime absoluteExpiration) where T : ICacheable;

        void Remove(string key);

        void CollectExpiredKeys();

        // Clear all cache items and establish the cache from settings again
        void Refresh();
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class CacheService : ICacheService
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<CacheService>();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly Func<IRohamConfigs> _rohamConfigsResolver;
        private readonly Func<ICacheProvider> _cacheProvider;
        private readonly object _memoryCacheLock = new object();
        private readonly object _cacheLock = new object();

        private readonly static object _collectTimestampLock = new object();
        private DateTime _lastCollectTimestamp = DateTime.UtcNow;
        private bool _collectKeysRunning = false;
        private readonly TimeSpan _collectKeysInterval = TimeSpan.FromMinutes(5); 

        private ICache _memoryCache;
        private ICache _cache;
        private readonly Dictionary<string, HashSet<string>> _hashMembers = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, CacheIndex> _cachIndexes = new Dictionary<string, CacheIndex>();


        public CacheService(
            Func<IRohamConfigs> rohamConfigsResolver,
            Func<ICacheProvider> cacheProvider)
        {
            _rohamConfigsResolver = rohamConfigsResolver;
            _cacheProvider = cacheProvider;
        }

        protected ICacheProvider CacheProvider => _cacheProvider();

        public ICache MemoryCache
        {
            get
            {
                if (_memoryCache == null)
                {
                    lock (_memoryCacheLock)
                    {
                        if (_memoryCache == null)
                        {
                            _memoryCache = CacheProvider.CreateCache(new CacheInfo(CacheProviders.Memory));
                        }
                    }
                }
                return _memoryCache;
            }
        }

        protected ICache Cache
        {
            get
            {
                if (_cache == null)
                {
                    lock (_cacheLock)
                    {
                        if (_cache == null)
                        {
                            var configs = _rohamConfigsResolver();
                            var cacheInfo = CacheProvider.CreateInfo(configs.CacheProvider, configs.CacheConnectionString);
                            _cache = CacheProvider.CreateCache(cacheInfo);
                        }
                    }
                }
                return _cache;
            }
        }
        
        public int CachedSetsCount => _hashMembers.Count;

        public T Get<T>(CacheKey key, Func<T> loadAction) where T : ICacheable
        {   
            return GetObject(key.Key, loadAction);
        }

        public T Get<T>(CacheIndex index, Func<T> loadAction) where T : ICacheable
        {
            T @object = default(T);                        
            CacheIndex cacheIndex;
            if (_cachIndexes.ContainsKey(index.IndexKey))
            {
                cacheIndex = _cachIndexes[index.IndexKey];                
            }
            else
            {
                using (rwLock.WriteScope())
                {
                    cacheIndex = new CacheIndex(index.Type, index.Name, index.Value);
                    _cachIndexes.Add(index.IndexKey, cacheIndex);
                }
            }

            if (cacheIndex.CacheKey != null)
            {
                @object = GetObject<T>(cacheIndex.CacheKey, loadAction);
            }
            else if (loadAction != null)
            {
                @object = loadAction();
                if (@object != null)
                {
                    Set(@object);
                }
            }

            if (@object != null)
            {
                using (rwLock.WriteScope())
                {
                    _cachIndexes[index.IndexKey].SetCacheKey(@object.CacheKey);
                }
            }

            return @object;
        }

        public void Set<T>(T @object, TimeSpan? slidingExpiration = null) where T : ICacheable
        {
            using (rwLock.WriteScope())
            {
                var key = @object.CacheKey.Key;
                Cache.Set(key, @object, slidingExpiration);
            }
        }

        public void Set<T>(T @object, DateTime absoluteExpiration) where T : ICacheable
        {
            using (rwLock.WriteScope())
            {
                var key = @object.CacheKey.Key;
                Cache.Set(key, @object, absoluteExpiration);
            }
        }

        public HashSet<T> GetHash<T>(string key, Func<HashSet<T>> loadAction) where T : ICacheable
        {
            HashSet<T> hashSet;
            using (rwLock.ReadScope())
            {   
                if (Cache.TryGetHashSet(key, out hashSet))
                {
                    return hashSet;
                }
                RemoveHashKey(key);
            }

            CheckAndCollectExpiredHashKeys();

            if (loadAction != null)
            {
                hashSet = loadAction();
                SetHash(key, hashSet);
            }

            return hashSet;
        }

        public void SetHash<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null) where T : ICacheable
        {
            Objects.Requires<NullReferenceException>(hashSet != null);

            using (rwLock.WriteScope())
            {
                Cache.SetHashSet(key, hashSet, slidingExpiration);
                AddHashMemberKeys(key, hashSet);
            }
        }

        public void SetHash<T>(string key, HashSet<T> hashSet, DateTime absoluteExpiration) where T : ICacheable
        {
            Objects.Requires<NullReferenceException>(hashSet != null);

            using (rwLock.WriteScope())
            {
                Cache.SetHashSet(key, hashSet, absoluteExpiration);
                AddHashMemberKeys(key, hashSet);
            }
        }

        public void Remove(string key)
        {
            using (rwLock.WriteScope())
            {      
                Cache.Remove(key);
                RemoveHashKey(key);
                RemoveIndex(key);
            }

            CheckAndCollectExpiredHashKeys();
        }

        public void CollectExpiredKeys()
        {
            CheckAndCollectExpiredHashKeys(true);
        }

        private T GetObject<T>(string key, Func<T> loadAction) where T : ICacheable
        {   
            T @object = default(T);
            using (rwLock.ReadScope())
            {
                if (Cache.TryGet(key, out @object))
                {
                    return @object;
                }
            }

            CheckAndCollectExpiredHashKeys();

            if (loadAction != null)
            {
                @object = loadAction();
                Set(@object);
            }

            return @object;
        }

        private void CheckAndCollectExpiredHashKeys(bool force = false)
        {   
            Action collectAction = () =>
            {
                if (_collectKeysRunning)
                {
                    return;
                }

                _collectKeysRunning = true;
                try
                {
                    Log.Debug("Collecting expired cached keys started.");
                    
                    RemoveExpiredHashKeys();

                    Log.Debug("Collecting expired cached keys finished.");
                }
                catch (Exception ex)
                {
                    Log.Error("CollectExpiredKeys failed", ex);
                }
                finally
                {
                    _collectKeysRunning = false;
                }
            };

            if (force)
            {
                collectAction();
            }
            else
            {
                var newTimestamp = _lastCollectTimestamp.AddMinutes(_collectKeysInterval.TotalMinutes);
                if (DateTime.UtcNow > newTimestamp)
                {
                    _lastCollectTimestamp = newTimestamp;
                    Task.Factory.StartNew(collectAction);
                }
            }
        }

        private void RemoveExpiredHashKeys()
        {
            using (rwLock.WriteScope())
            {
                var keysToRemove = new List<string>();
                foreach (var hashKey in _hashMembers.Keys)
                {
                    if (!Cache.Exists(hashKey))
                    {
                        keysToRemove.Add(hashKey);
                    }
                }
                keysToRemove.ForEach(k => _hashMembers.Remove(k));
            }
        }

        private void RemoveExpiredIndexKeys()
        {
            using (rwLock.WriteScope())
            {
                var keysToRemove = new List<string>();
                foreach (var indx in _cachIndexes.Values)
                {
                    if (indx.Key == null)
                    {
                        if (!Cache.Exists(indx.IndexKey))
                        {
                            keysToRemove.Add(indx.IndexKey);
                        }
                    }
                    else
                    {
                        if (!Cache.Exists(indx.Key) && !Cache.Exists(indx.IndexKey))
                        {
                            keysToRemove.Add(indx.IndexKey);
                        }
                    }
                }
                keysToRemove.ForEach(k => _cachIndexes.Remove(k));
            }
        }

        private void AddHashMemberKeys<T>(string hashKey, HashSet<T> hashSet) where T : ICacheable
        {
            if (hashSet == null)
            {
                return;
            }

            var memberKeys = new HashSet<string>(hashSet.Select(m => m.Key));
            if (!_hashMembers.ContainsKey(hashKey))
            {
                _hashMembers.Add(hashKey, memberKeys);
            }
            else
            {
                _hashMembers[hashKey] = memberKeys;
            }
        }

        private void RemoveHashKey(string key)
        {   
            if (_hashMembers.ContainsKey(key))
            {
                _hashMembers.Remove(key);
            }
            else
            {
                var hashKeysToRemove = new List<string>();
                foreach (var entry in _hashMembers)
                {   
                    if (entry.Value.Contains(key))
                    {
                        hashKeysToRemove.Add(entry.Key);
                    }
                }

                foreach(var hashKey in hashKeysToRemove)
                {
                    Cache.Remove(hashKey);
                    _hashMembers.Remove(hashKey);
                }
            }
        }

        private void RemoveIndex(string key)
        {
            if (_cachIndexes.ContainsKey(key))
            {
                _cachIndexes.Remove(key);
                return;
            }

            var removingIndexes = _cachIndexes.Where(entry => entry.Value.CacheKey == key).ToList();
            foreach (var indx in removingIndexes)
            {
                _cachIndexes.Remove(indx.Key);
            }
        }

        public void Refresh()
        {
            if (_cache != null)
            {
                lock (_cacheLock)
                {
                    if (_cache != null)
                    {
                        _cache.ClearAll();
                        _cache = null;
                    }                    
                }
            }
        }
    }
}
