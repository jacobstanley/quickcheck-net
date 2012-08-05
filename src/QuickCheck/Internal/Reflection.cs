using System.Reflection;

namespace QuickCheck.Internal
{
    public static class Reflection
    {
        public static ITestable Testable(object instance, MethodInfo method)
        {
            return ReflectionTestable.Testable(instance, method);
        }

        public static ITestable Testable(object test)
        {
            return ReflectionTestable.Testable(test);
        }

        public static string Show(object instance)
        {
            return ReflectionData.CreateFrom(instance).ToString();
        }

        public static Data Data(object instance)
        {
            return ReflectionData.CreateFrom(instance);
        }
    }
}
