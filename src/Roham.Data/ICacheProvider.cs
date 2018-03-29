using Roham.Lib.Caches;

namespace Roham.Data
{
    public enum CacheProviders
    {
        Memory,
        Redis
    }

    public interface ICacheProvider
    {   
        bool TryConnect(CacheInfo cacheInfo, out string errorMessage);
        bool TryConnect(CacheProviders provider, string connectionString, out string errorMessage);
        string BuildConnectionString(CacheInfo cacheInfo);        
        CacheInfo CreateInfo(CacheProviders provider, string connectionString);
        ICache CreateCache(CacheInfo cacheInfo);
    }
}
