using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuickCheck.NUnit;

namespace QuickCheck.Examples
{
    public class MathProperties : TestProperties
    {
        // Fails due to integer overflow
        public void positive_squares(int x)
        {
            Assert.That(x * x, Is.GreaterThanOrEqualTo(0));
        }

        // Fails due to integer overflow
        public void square_is_greater_than_value(int x)
        {
            if (x > 1) Assert.That(x * x, Is.GreaterThan(x));
        }

        public void multiplication_commutes(int x, int y)
        {
            AreEqual(x * y, y * x);
        }

        public void reverse_reverse_is_identity(int[] xs)
        {
            AreEqual(xs, xs.Reverse().Reverse());
        }

        public void union_commutes(HashSet<int> xs, HashSet<int> ys)
        {
            // copy xs as UnionWith does a destructive update
            var xs1 = new HashSet<int>(xs);

            xs.UnionWith(ys);
            ys.UnionWith(xs1);

            // TODO: Implement type class style data deconstruction
            AreEqual(Sort(xs), Sort(ys));
        }
    }
}
