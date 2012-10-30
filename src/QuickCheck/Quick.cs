using System;
using System.Reflection;
using QuickCheck.Internal;
using QuickCheck.Random;

namespace QuickCheck
{
    public static class Quick
    {
        private static readonly IRandomFactory s_RandomFactory;
        private static readonly GeneratorContainer s_Generators;

        static Quick()
        {
            s_RandomFactory = new MwcFactory();
            s_Generators = new GeneratorContainer();

            Register(typeof(Quick).Assembly);
        }

        public static IGenerator<T> Generator<T>()
        {
            return s_Generators.Instance<T>();
        }

        public static void Register(Assembly assembly)
        {
            s_Generators.Register(assembly);
        }

        public static void Register(Type type)
        {
            s_Generators.Register(type);
        }

        public static Result Test(ITestable test)
        {
            const int maxSuccess = 250;
            const int maxSize = 100;

            int i;

            for (i = 0; i < maxSuccess; i++)
            {
                ulong seed;
                IRandom random = s_RandomFactory.NewRandom(out seed);
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
            IRandom random = s_RandomFactory.NewRandom(seed);
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
