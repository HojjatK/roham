using System;
using System.Linq;
using System.Collections.Generic;
using StackExchange.Redis;
using Newtonsoft.Json;
using Roham.Data;
using Roham.Lib.Caches;

namespace Roham.Caching
{   
    public class RedisCache : Cache<StackExchange.Redis.IDatabase>
    {
        private readonly object @lock = new object();
        private ConnectionMultiplexer _connection;
        private StackExchange.Redis.IDatabase _cache;
        private readonly CacheInfo _cacheInfo;        

        public RedisCache(CacheInfo cacheInfo)
        {
            _cacheInfo = cacheInfo;
        }
        
        protected StackExchange.Redis.IDatabase CacheDatabase
        {
            get
            {
                if (_cache == null)
                {   
                    lock(@lock)
                    {
                        if (_cache == null)
                        {
                            var options = CreateOptions(_cacheInfo);
                            _connection = ConnectionMultiplexer.Connect(options);                            
                            _cache = _connection.GetDatabase();                            
                        }
                    }
                }
                return _cache;
            }
        }

        public override bool Exists(string key)
        {
            return CacheDatabase.KeyExists(key);
        }

        public override bool TryGet<T>(string key, out T @object)
        {
            @object = default(T);
            try
            {
                var cacheObject = CacheDatabase.StringGet(key);
                if (!cacheObject.HasValue)
                {   
                    return false;
                }

                @object = Convert<T>(cacheObject);
            }
            catch
            {   
                return false;
            }

            return true;
        }

        public override void Set<T>(string key, T @object, TimeSpan? slidingExpiration = null)
        {   
            SetObject(key, @object);
            CacheDatabase.KeyExpire(key, slidingExpiration ?? CacheDuration);
        }

        public override void Set<T>(string key, T @object, DateTime absoluteExpiration)
        {
            SetObject(key, @object);
            CacheDatabase.KeyExpire(key, absoluteExpiration);
        }

        public override bool TryGetHashSet<T>(string key, out HashSet<T> hashSet) 
        {
            hashSet = null;
            try
            {
                var hashEntries = CacheDatabase.HashGetAll(key);
                if (hashEntries == null || hashEntries.IsEmpty())
                {   
                    return false;
                }

                hashSet = new HashSet<T>();
                foreach (var data in hashEntries)
                {
                    hashSet.Add(Convert<T>(data.Value));
                }
            }
            catch
            {
                hashSet = null;
                return false;
            }

            return true;
        }

        public override void SetHashSet<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null) 
        {
            SetHashSet(key, hashSet);
            CacheDatabase.KeyExpire(key, slidingExpiration ?? CacheDuration);
        }

        public override void SetHashSet<T>(string key, HashSet<T> hashSet, DateTime absoluteExpiration) 
        {
            SetHashSet(key, hashSet);
            CacheDatabase.KeyExpire(key, absoluteExpiration);
        }

        public override void Remove(string key)
        {
            CacheDatabase.KeyDelete(key);
        }

        public override void ClearAll()
        {
            if (_cacheInfo != null && _connection != null)
            {
                var server =  _connection.GetServer(_cacheInfo.Host, _cacheInfo.Port);
                var keys = server.Keys();
                foreach (var key in keys)
                {   
                    _cache.KeyDelete(key);
                }
            }
        }

        public static CacheInfo CreateInfo(string connectionString)
        {
            return CreateInfo(ConfigurationOptions.Parse(connectionString));
        }

        public static ConfigurationOptions CreateOptions(CacheInfo cacheInfo)
        {
            var options = new ConfigurationOptions();
            options.EndPoints.Add(cacheInfo.Host, cacheInfo.Port);
            options.Password = cacheInfo.Password;
            options.Ssl = cacheInfo.Ssl;
            options.DefaultDatabase = cacheInfo.Db;
            if (cacheInfo.ConnectTimeout.HasValue)
            {
                options.ConnectTimeout = cacheInfo.ConnectTimeout.Value;
            }
            if (cacheInfo.SendTimeout.HasValue)
            {
                options.SyncTimeout = cacheInfo.SendTimeout.Value;
            }
            if (cacheInfo.ReceiveTimeout.HasValue)
            {
                options.ResponseTimeout = cacheInfo.ReceiveTimeout.Value;
            }
            return options;
        }

        public static CacheInfo CreateInfo(ConfigurationOptions options)
        {
            string hostName = null;
            int port = 0;
            var endPoint = options.EndPoints.FirstOrDefault() as System.Net.DnsEndPoint;
            if (endPoint != null)
            {
                hostName = endPoint.Host;
                port = endPoint.Port;
            }
            else
            {
                var ipEndPoint = options.EndPoints.FirstOrDefault() as System.Net.IPEndPoint;
                hostName = ipEndPoint?.Address.ToString();
                port = ipEndPoint?.Port ?? 0;
            }            
            
            return new CacheInfo(
                CacheProviders.Redis,
                hostName,
                port,
                options.Ssl,
                options.Password,
                options.DefaultDatabase,
                options.ClientName,
                options.ConnectTimeout,
                options.SyncTimeout,
                options.ResponseTimeout,
                null,
                null);
        }

        private T Convert<T>(string data)
        {   
            return JsonConvert.DeserializeObject<T>(data,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        private string ConvertToString<T>(T @object)
        {
            return JsonConvert.SerializeObject(@object, 
                Formatting.None, 
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        private void SetObject<T>(string key, T @object)
        {
            
            var data = ConvertToString(@object);
            CacheDatabase.StringSet(key, data);
        }

        private void SetHashSet<T>(string key, HashSet<T> hashSet) where T : IKeyed
        {   
            var hashEntries = hashSet == null 
                ? new HashEntry[0] 
                : hashSet.Select(v => new HashEntry(v.Key, ConvertToString(v))).ToArray();
            CacheDatabase.HashSet(key, hashEntries);
        }
    }
}
