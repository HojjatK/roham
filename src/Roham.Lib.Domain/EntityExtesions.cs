using System.Collections.Generic;

namespace Roham.Lib.Domain
{
    public static class EntityExtesions
    {
        public static ICollection<T> LazySet<T>(this Identifiable identifiable, ref ICollection<T> set) where T : class
        {
            if (set == null)
            {
                set = new HashSet<T>();
            }
            return set;
        }

        public static ICollection<T> AsSet<T>(this ICollection<T> collection) where T : class
        {
            if (collection.GetType().IsArray)
            {
                var hashSet = new HashSet<T>();
                foreach (var item in collection)
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
            return collection;
        }
    }
}
