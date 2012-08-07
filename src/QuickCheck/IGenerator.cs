using QuickCheck.Random;

namespace QuickCheck
{
    public interface IGenerator<out T>
    {
        T Arbitrary(IRandom gen, int size);
    }
}
