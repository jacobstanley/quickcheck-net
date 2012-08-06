using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using QuickCheck.Internal;

namespace QuickCheck.NUnit
{
    public abstract class TestProperties : Property
    {
    }

    public abstract class Property
    {
        internal Property()
        {
        }

        protected void IsTrue(bool prop)
        {
            Assert.IsTrue(prop);
        }

        protected void AreEqual(object x, object y)
        {
            Assert.AreEqual(Reflection.Data(x), Reflection.Data(y));
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
