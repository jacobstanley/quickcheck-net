using System.Linq;
using NUnit.Framework;
using QuickCheck.NUnit;

namespace QuickCheck.Examples
{
    public class MathProperties : TestProperties
    {
        // TODO: Should allow checking bool return values?
        //public bool multiplication_commutivity(int x, int y)
        //{
        //    return x * y == y * x;
        //}

        public void multiplication_commutivity(int x, int y)
        {
            Assert.AreEqual(x * y, y * x);
        }

        public void reverse_reverse_is_identity(int[] xs)
        {
            CollectionAssert.AreEqual(xs, xs.Reverse().Reverse());
        }
    }
}
