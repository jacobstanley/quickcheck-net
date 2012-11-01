using NUnit.Framework;
using QuickCheck.Internal;

namespace QuickCheck.NUnit
{
    public static class NUnitExtensions
    {
        public static void AssertEmpty(this DataDiff diff)
        {
            if (diff.IsEmpty)
            {
                return;
            }

            Assert.Fail("Expected no differences\nBut was " + diff);
        }
    }
}