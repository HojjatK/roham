using System;

namespace Roham.Lib.Domain.Cache
{
    public class CacheKey
    {
        // For serialisation
        public CacheKey()
        {
        }

        public CacheKey(Type type, string propertyName, object propertyValue, bool isPrimary = true)
        {
            Type = type;
            IsPrimary = isPrimary;
            Key = $"{type.Name}-{propertyName}:{propertyValue}";
        }

        public CacheKey(Type type, string keyVal, bool isPrimary = true)
        {
            Type = type;
            IsPrimary = isPrimary;
            Key = $"{type.Name}-{keyVal}";
        }

        public string Key { get; }
        public bool IsPrimary { get; }
        public Type Type { get; }

        public override string ToString()
        {
            return Key;
        }

        public override bool Equals(object obj)
        {
            var objCacheKey = obj as CacheKey;
            if (objCacheKey == null)
            {
                return false;
            }

            return Key.Equals(objCacheKey.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(CacheKey key, CacheKey otherKey)
        {
            if (ReferenceEquals(key, otherKey))
            {
                return true;
            }

            if ((object)key == null || (object)otherKey == null)
            {
                return false;
            }

            return key.Equals(otherKey);
        }

        public static bool operator !=(CacheKey key, CacheKey otherKey)
        {
            if (ReferenceEquals(key, otherKey))
            {
                return false;
            }
            if (key == null || otherKey == null)
            {
                return true;
            }

            return !key.Equals(otherKey);
        }

        public static CacheKey New<T, V>(string propertyName, V propertyValue)
        {
            return new CacheKey(typeof(T), propertyName, propertyValue);
        }

        public static CacheIndex NewIndex<T, V>(string indexKey, V indexValue)
        {
            return new CacheIndex(typeof(T), indexKey, indexValue);
        }
    }

    public class CacheIndex
    {
        // For serialisation
        public CacheIndex()
        {
        }

        public CacheIndex(Type type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
            IndexKey = $"{type.Name}-{name}:{value}";
        }

        public Type Type { get; }        
        public string Name { get; }
        public object Value { get; }

        public string IndexKey { get; }
        public string CacheKey { get; private set; }
        public string Key => $"{IndexKey}->{CacheKey??"(null)"}";

        public void SetCacheKey(CacheKey cacheKey)
        {
            if (cacheKey == null)
            {
                throw new NullReferenceException(nameof(CacheKey));
            }
            if (Type.FullName != cacheKey.Type.FullName)
            {
                throw new InvalidCastException($"CacheKey type:{cacheKey.GetType()} and CacheIndex Type:{Type} are not the same");
            }
            CacheKey = cacheKey.Key;
        }
    }
}
