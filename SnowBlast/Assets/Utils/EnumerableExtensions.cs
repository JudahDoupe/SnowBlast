using System.Collections;
using System.Collections.Generic;

namespace Assets.Utils
{
    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> self, string separator = "") => string.Join(separator, self);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self) => new HashSet<T>(self);
    }
}