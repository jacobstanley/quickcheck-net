using System;
using System.Reflection;
using QuickCheck.Random;
using QuickCheck.SimpleInjector;
using SimpleInjector;

namespace QuickCheck
{
    public static class Quick
    {
        private static readonly IRandomFactory s_Factory;
        private static readonly Container s_Container;

        static Quick()
        {
            s_Factory = new MwcFactory();
            s_Container = new Container();

            Register(typeof(Quick).Assembly);
        }

        public static IGenerator<T> Generator<T>()
        {
            return s_Container.GetInstance<IGenerator<T>>();
        }

        public static void Register(Assembly assembly)
        {
            s_Container.RegisterManySingles(typeof(IGenerator<>), assembly);
        }

        public static Result Test(ITestable test)
        {
            const int maxSuccess = 250;
            const int maxSize = 100;

            int i;

            for (i = 0; i < maxSuccess; i++)
            {
                ulong seed;
                IRandom random = s_Factory.NewRandom(out seed);
                int size = i % maxSize + 1;

                TestResult result = test.RunTest(random, size);

                if (result.IsFailure)
                {
                    return Result.Failure(i + 1, seed, size, result.Args, result.Error);
                }
            }

            return Result.Success(i + 1);
        }

        public static TestResult Replay(ulong seed, int size, ITestable test)
        {
            Console.WriteLine(
                "Replaying test with seed: ({0}, {1})", seed, size);
            IRandom random = s_Factory.NewRandom(seed);
            return test.RunTest(random, size);
        }

        public static void Check<A>(Action<A> test)
        {
            Test(new TestableAction<A>(test)).ThrowError();
        }

        public static void Check<A, B>(Action<A, B> test)
        {
            Test(new TestableAction<A, B>(test)).ThrowError();
        }

        public static void Check<A>(ulong seed, int size, Action<A> test)
        {
            Replay(seed, size, new TestableAction<A>(test)).ThrowError();
        }

        public static void Check<A, B>(ulong seed, int size, Action<A, B> test)
        {
            Replay(seed, size, new TestableAction<A, B>(test)).ThrowError();
        }
    }
}
