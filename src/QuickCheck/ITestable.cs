using QuickCheck.Random;

namespace QuickCheck
{
    public interface ITestable
    {
        TestResult RunTest(IRandom gen, int size);
    }
}
