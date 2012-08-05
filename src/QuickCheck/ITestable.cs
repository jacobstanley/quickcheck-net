namespace QuickCheck
{
    public interface ITestable
    {
        TestResult RunTest(Generator gen, int size);
    }
}
