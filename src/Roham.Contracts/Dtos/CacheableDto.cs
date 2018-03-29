using Roham.Lib.Domain.Cache;

namespace Roham.Contracts.Dtos
{
    public abstract class CachableDto : ICacheable
    {
        public abstract CacheKey CacheKey { get; }
        public string Key => CacheKey?.Key;
    }
}
