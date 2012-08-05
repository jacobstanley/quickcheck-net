namespace QuickCheck
{
    public interface IGenerator<out T>
    {
        T Arbitrary(Generator gen, int size);
    }
}
