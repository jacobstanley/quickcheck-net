using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QuickCheck.Internal
{
    internal static class ReflectionTestable
    {
        public static ITestable Testable(object instance, MethodInfo method)
        {
            Type[] args = method
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            Type actionType = Expression.GetActionType(args);
            Delegate action = Delegate.CreateDelegate(actionType, instance, method);

            return Testable(action);
        }

        public static ITestable Testable(object test)
        {
            Type testable;
            Type type = test.GetType();

            if (!type.IsGenericType)
            {
                s_Testables.TryGetValue(type, out testable);
            }
            else if (s_Testables.TryGetValue(type.GetGenericTypeDefinition(), out testable))
            {
                testable = testable.MakeGenericType(type.GetGenericArguments());
            }

            if (testable == null)
            {
                throw new NotSupportedException("Could figure out how to test: " + type);
            }

            return (ITestable)Activator.CreateInstance(testable, test);
        }

        private static readonly Dictionary<Type, Type> s_Testables = TestablesLookup();

        private static Dictionary<Type, Type> TestablesLookup()
        {
            var lookup = new Dictionary<Type, Type>();

            var testables = typeof(Reflection).Assembly
                .GetTypes()
                .Where(IsTestable)
                .SelectMany(type => SingleParamCtors(type)
                    .Select(ctor => new { type, ctor }));

            foreach (var testable in testables)
            {
                if (testable.type.IsGenericType && !testable.ctor.IsGenericType)
                {
                    // not enough information to construct such types
                    continue;
                }

                if (testable.type.IsGenericType)
                {
                    var gctor = testable.ctor.GetGenericTypeDefinition();
                    var gtype = testable.type.GetGenericTypeDefinition();

                    lookup.Add(gctor, gtype);
                }
                else
                {
                    lookup.Add(testable.ctor, testable.type);
                }
            }

            return lookup;
        }

        private static bool IsTestable(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ITestable));
        }

        private static IEnumerable<Type> SingleParamCtors(Type type)
        {
            return type
                .GetConstructors()
                .Select(c => c.GetParameters())
                .Where(ps => ps.Length == 1)
                .Select(ps => ps[0].ParameterType)
                .ToArray();
        }
    }
}
