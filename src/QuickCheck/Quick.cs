using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace QuickCheck
{
    public static class Quick
    {
        private static readonly Random s_RandomSeed;
        private static readonly Dictionary<Type, object> s_Generators;
        private static readonly Dictionary<Type, Type> s_GeneratorTypes;

        static Quick()
        {
            s_RandomSeed = new Random();
            s_Generators = new Dictionary<Type, object>();
            s_GeneratorTypes = new Dictionary<Type, Type>();

            Register(typeof(Generator).Assembly);
        }

        public static Generator Generator()
        {
            return Generator(s_RandomSeed.Next());
        }

        public static Generator Generator(int seed)
        {
            return new Generator(seed);
        }

        public static IGenerator<T> Generator<T>()
        {
            object generator;

            if (!s_Generators.TryGetValue(typeof(T), out generator))
            {
                Type generatorType = GeneratorType(typeof(T));
                generator = Activator.CreateInstance(generatorType);

                s_Generators.Add(typeof(T), generator);
            }

            return (IGenerator<T>)generator;
        }

        public static void Register(Assembly assembly)
        {
            foreach (Type generator in assembly.GetTypes())
            {
                Register(generator);
            }
        }

        public static void Register(Type generator)
        {
            foreach (Type generatable in GeneratableTypes(generator))
            {
                s_GeneratorTypes.Add(
                    GenericDef(generatable),
                    GenericDef(generator));
            }
        }

        public static void Register<A, V>()
            where A : IGenerator<V>
        {
            Register(typeof(A));
        }

        private static Type GenericDef(Type type)
        {
            if (type.IsArray)
            {
                return typeof(Array);
            }

            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition();
            }

            return type;
        }

        private static IEnumerable<Type> GeneratableTypes(Type type)
        {
            return type.GetInterfaces()
                .Where(IsArbitrary)
                .Select(x => x.GetGenericArguments().First())
                .ToArray();
        }

        private static bool IsArbitrary(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IGenerator<>);
            }

            return false;
        }

        private static Type GeneratorType(Type value)
        {
            Type generic = GenericDef(value);

            Type generator;
            if (!s_GeneratorTypes.TryGetValue(generic, out generator))
            {
                // TODO: C# names for types
                throw new InvalidOperationException(
                    "Could not find a generator for " + generic.Name + ", please register one.");
            }

            if (generic == typeof(Array))
            {
                return generator.MakeGenericType(value.GetElementType());
            }

            if (generator.IsGenericTypeDefinition)
            {
                return generator.MakeGenericType(value.GetGenericArguments());
            }

            return generator;
        }

        private static void Test(TestAction test)
        {
            const int maxSuccess = 250;
            const int maxSize = 100;

            int seed = 0;
            int size = 0;

            try
            {
                for (int i = 0; i < maxSuccess; i++)
                {
                    seed = s_RandomSeed.Next();
                    size = i % maxSize + 1;
                    test(Generator(seed), size);
                }

                Console.WriteLine(
                    "OK, passed {0} tests.", maxSuccess);
            }
            catch (Exception)
            {
                Console.WriteLine(
                    "Test failed, use Quick.Check({0}, {1}, {{your function...}}) to reproduce.",
                    seed, size);
                throw;
            }
        }

        private static void Replay(int seed, int size, TestAction test)
        {
            Console.WriteLine(
                "Replaying test with seed: ({0}, {1})", seed, size);
            test(Generator(seed), size);
        }

        public static void Check<A>(Action<A> test)
        {
            Test(Testable(test));
        }

        public static void Check<A, B>(Action<A, B> test)
        {
            Test(Testable(test));
        }

        //public static void Check<A>(Func<A, bool> test)
        //{
        //    Test(Testable(test));
        //}

        //public static void Check<A, B>(Func<A, B, bool> test)
        //{
        //    Test(Testable(test));
        //}

        public static void Check<A>(int seed, int size, Action<A> test)
        {
            Replay(seed, size, Testable(test));
        }

        public static void Check<A, B>(int seed, int size, Action<A, B> test)
        {
            Replay(seed, size, Testable(test));
        }

        //public static void Check<A>(int seed, int size, Func<A, bool> test)
        //{
        //    Replay(seed, size, Testable(test));
        //}

        //public static void Check<A, B>(int seed, int size, Func<A, B, bool> test)
        //{
        //    Replay(seed, size, Testable(test));
        //}

        private delegate void TestAction(Generator gen, int size);

        private static TestAction Testable<A>(Action<A> action)
        {
            return (gen, size) => action(
                gen.Arbitrary<A>(size));
        }

        private static TestAction Testable<A, B>(Action<A, B> action)
        {
            return (gen, size) =>
            {
                A x = gen.Arbitrary<A>(size);
                B y = gen.Arbitrary<B>(size);

                //Verbose
                //Console.WriteLine(x + ", " + y);

                try
                {
                    action(x, y);
                }
                catch (Exception exception)
                {
                    // TODO: Replace this with a Result return value
                    throw new TargetInvocationException(exception);
                }
            };
        }
    }
}
