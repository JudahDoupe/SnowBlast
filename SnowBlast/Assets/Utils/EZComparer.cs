using System;
using System.Collections.Generic;

namespace Assets.Utils
{
    public class EZComparer<T>: IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> Comparer;
        private readonly Func<T, int> Hash;

        public EZComparer(Func<T, T, bool> comparer, Func<T, int> hash)
        {
            Comparer = comparer;
            Hash = hash;
        }

        public bool Equals(T x, T y)
        {
            return Comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return Hash(obj);
        }
    }
}