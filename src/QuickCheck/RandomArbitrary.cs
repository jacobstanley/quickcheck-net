using QuickCheck.Random;

namespace QuickCheck
{
    public static class RandomArbitrary
    {
        public static T Arbitrary<T>(this IRandom random, int size)
        {
            return Quick.Generator<T>().Arbitrary(random, size);
        }
    }
}
