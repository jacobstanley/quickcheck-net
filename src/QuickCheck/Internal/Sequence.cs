using System.Collections.Generic;
using System.Linq;

namespace QuickCheck.Internal
{
    internal static class Sequence
    {
        public static bool Equals<T>(IEnumerable<T> xs, IEnumerable<T> ys)
        {
            return xs.SequenceEqual(ys);
        }

        public static int GetHashCode<T>(IEnumerable<T> xs)
        {
            return xs.Aggregate(19, (hash, x) => hash * 31 + x.GetHashCode());
        }
    }
}
