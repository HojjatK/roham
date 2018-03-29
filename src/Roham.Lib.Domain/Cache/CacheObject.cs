using Roham.Lib.Caches;

namespace Roham.Lib.Domain.Cache
{
    public interface ICacheable : IKeyed
    {
        CacheKey CacheKey { get; }        
    }

    public class CacheObject<T> : ICacheable
    {
        public CacheObject(string key, T value)
        {
            CacheKey = new CacheKey(typeof(T), key);
            Object = value;
        }

        public CacheKey CacheKey { get; }
        public string Key => CacheKey?.Key;        
        public T Object { get; set; }
    }
}
