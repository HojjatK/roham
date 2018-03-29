using Roham.Data;
using System;
using Roham.Lib.Caches;
using StackExchange.Redis;
using Roham.Lib.Ioc;

namespace Roham.Caching
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class CacheProvider : ICacheProvider
    {
        public string BuildConnectionString(CacheInfo cacheInfo)
        {
            Objects.Requires(cacheInfo != null, () => new ArgumentNullException(nameof(CacheInfo)));

            switch (cacheInfo.Provider)
            {
                case CacheProviders.Memory:
                    return string.Empty;
                case CacheProviders.Redis:
                    return RedisCache.CreateOptions(cacheInfo).ToString();
                default:
                    throw new NotSupportedException($"{cacheInfo.Provider} cache provider is not supported");
            }
        }

        public bool TryConnect(CacheInfo cacheInfo, out string errorMessage)
        {
            errorMessage = "";            
            if (cacheInfo.Provider == CacheProviders.Redis)
            {   
                try
                {
                    var options = RedisCache.CreateOptions(cacheInfo);
                    ConnectionMultiplexer.Connect(options);
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
            return true;
        }

        public bool TryConnect(CacheProviders provider, string connectionString, out string errorMessage)
        {
            errorMessage = "";
            if (provider == CacheProviders.Redis)
            {
                try
                {
                    ConnectionMultiplexer.Connect(connectionString);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
            return true;
        }

        public CacheInfo CreateInfo(CacheProviders provider, string connectionString)
        {
            switch (provider)
            {
                case CacheProviders.Memory:
                    return new CacheInfo(provider);
                case CacheProviders.Redis:
                    return RedisCache.CreateInfo(connectionString);                    
                default:
                    throw new NotSupportedException($"{provider} cache provider is not supported");
            }
        }

        public ICache CreateCache(CacheInfo cacheInfo)
        {
            switch (cacheInfo.Provider)
            {
                case CacheProviders.Memory:
                    return new MemCache();
                case CacheProviders.Redis:
                    return new RedisCache(cacheInfo);
                default:
                    throw new NotSupportedException($"{cacheInfo.Provider} cache provider is not supported");
            }
        }
    }
}
