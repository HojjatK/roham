using System;
using System.Collections.Generic;

namespace Roham.Lib.Caches
{
    public interface IKeyed
    {
        string Key { get; }
    }

    public interface ICache
    {
        TimeSpan CacheDuration { get; }

        bool Exists(string key);
        bool TryGet<T>(string key, out T @object);
        void Set<T>(string key, T @object, TimeSpan? slidingExpiration = null);
        void Set<T>(string key, T @object, DateTime absoluteExpiration);

        bool TryGetHashSet<T>(string key, out HashSet<T> hashSet) where T : IKeyed;
        void SetHashSet<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null) where T : IKeyed;
        void SetHashSet<T>(string key, HashSet<T> hashSet, DateTime absoluteExpiration) where T : IKeyed;

        void Remove(string key);

        void ClearAll();
    }

    public abstract class Cache<TCache> : ICache
    {
        private readonly static TimeSpan DefaultSlidingExpiration = new TimeSpan(0, 30, 0);

        public Cache() : this(DefaultSlidingExpiration) { }

        public Cache(TimeSpan duration)
        {
            CacheDuration = duration;
        }

        public TimeSpan CacheDuration { get; }

        public abstract bool Exists(string key);

        public abstract bool TryGet<T>(string key, out T @object);

        public abstract void Set<T>(string key, T @object, TimeSpan? slidingExpiration = null);

        public abstract void Set<T>(string key, T @object, DateTime absoluteExpiration);

        public abstract bool TryGetHashSet<T>(string key, out HashSet<T> hashSet) where T : IKeyed;

        public abstract void SetHashSet<T>(string key, HashSet<T> hashSet, TimeSpan? slidingExpiration = null) where T : IKeyed;

        public abstract void SetHashSet<T>(string key, HashSet<T> hashSet, DateTime absoluteExpiration) where T : IKeyed;

        public abstract void Remove(string key);

        public abstract void ClearAll();
    }
}
