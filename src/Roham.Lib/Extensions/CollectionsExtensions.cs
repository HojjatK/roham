using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        public static void ForEach<T>(this ICollection<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static void RemoveAny<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                collection.Remove(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> items)
        {
            return !items.Any();
        }

        public static bool IsUnique<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelector)
        {
            return !items.GroupBy(keySelector).Any(g => g.Skip(1).Any());
        }

        public static bool IsUnique<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return !items.GroupBy(keySelector, comparer).Any(g => g.Skip(1).Any());
        }
    }
}
