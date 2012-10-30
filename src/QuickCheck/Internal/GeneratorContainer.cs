using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuickCheck.Internal
{
    public class GeneratorContainer
    {
        private readonly Dictionary<Type, object> m_Generators;
        private readonly Dictionary<Type, Type> m_GeneratorTypes;

        public GeneratorContainer()
        {
            m_Generators = new Dictionary<Type, object>();
            m_GeneratorTypes = new Dictionary<Type, Type>();
        }

        public IGenerator<T> Instance<T>()
        {
            object generator;

            if (!m_Generators.TryGetValue(typeof(T), out generator))
            {
                Type generatorType = GeneratorType(typeof(T));
                generator = Activator.CreateInstance(generatorType);

                m_Generators.Add(typeof(T), generator);
            }

            return (IGenerator<T>)generator;
        }

        public void Register(Assembly assembly)
        {
            foreach (Type generator in assembly.GetTypes())
            {
                Register(generator);
            }
        }

        public void Register(Type generatorType)
        {
            foreach (Type generatable in GeneratableTypes(generatorType))
            {
                m_GeneratorTypes.Add(
                    GenericDef(generatable),
                    GenericDef(generatorType));
            }
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
                .Where(IsGenerator)
                .Select(x => x.GetGenericArguments().First())
                .ToArray();
        }

        private static bool IsGenerator(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IGenerator<>);
            }

            return false;
        }

        private Type GeneratorType(Type value)
        {
            Type generic = GenericDef(value);

            Type generator;
            if (!m_GeneratorTypes.TryGetValue(generic, out generator))
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
    }
}
