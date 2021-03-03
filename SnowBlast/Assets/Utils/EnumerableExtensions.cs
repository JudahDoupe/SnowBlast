using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Utils
{
    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> self, string separator = "") => string.Join(separator, self);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self) => new HashSet<T>(self);

        public static T MinBy<T>(this IEnumerable<T> self, Func<T, float> action)
        {
            return self
                .Select(item => new {item, value = action(item)})
                .Aggregate((current, next) => current.value <= next.value ? current : next)
                .item;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
        {
            return self.TryGetValue(key, out var value) ? value : default;
        }
    }
}