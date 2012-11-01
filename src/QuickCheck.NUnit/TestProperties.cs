using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using QuickCheck.Internal;

namespace QuickCheck.NUnit
{
    [TestFixture] // TestFixture attribute required for NUnit 2.5
    public abstract class TestProperties : Property
    {
    }

    public abstract class Property
    {
        internal Property()
        {
        }

        protected DataDiff Diff(object x, object y)
        {
            return Reflection.Diff(x, y);
        }

        protected IEnumerable<T> Sort<T>(IEnumerable<T> list)
        {
            return list.OrderBy(x => x);
        }

        [TestCaseSource("GetProperties")]
        public void Check(TestProperty property)
        {
            property.Check(this).ThrowError();
        }

        internal TestProperty[] GetProperties()
        {
            const BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

            var type = GetType();
            var methods = Enumerable.Empty<MethodInfo>();

            while (type != typeof(Property) && type != null)
            {
                methods = methods.Concat(type.GetMethods(bindingFlags));
                type = type.BaseType;
            }

            return methods
                .Select(x => new TestProperty(x))
                .ToArray();
        }
    }
}
