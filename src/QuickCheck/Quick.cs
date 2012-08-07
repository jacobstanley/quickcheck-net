using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QuickCheck.Random;

namespace QuickCheck
{
    public static class Quick
    {
        private static readonly IRandomFactory s_Factory;
        private static readonly Dictionary<Type, object> s_Generators;
        private static readonly Dictionary<Type, Type> s_GeneratorTypes;

        static Quick()
        {
            s_Factory = new MwcFactory();
            s_Generators = new Dictionary<Type, object>();
            s_GeneratorTypes = new Dictionary<Type, Type>();

            Register(typeof(Quick).Assembly);
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
            if (type.GetConstructors().All(x => x.GetParameters().Length != 0))
            {
                return new Type[0];
            }

            return type
                .GetInterfaces()
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
