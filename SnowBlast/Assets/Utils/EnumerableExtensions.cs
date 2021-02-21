using System.Collections;
using System.Collections.Generic;

namespace Assets.Utils
{
    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> self, string separator = "") => string.Join(separator, self);
    }
}