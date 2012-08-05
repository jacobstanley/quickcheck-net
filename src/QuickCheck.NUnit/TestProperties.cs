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
            var type = GetType();
            var methods = Enumerable.Empty<MethodInfo>();

            while (type != typeof(Property) && type != null)
            {
                var declared = type.GetMethods(
                    BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.DeclaredOnly);

                methods = methods.Concat(declared);
                type = type.BaseType;
            }

            return methods
                .Select(x => new TestProperty(x))
                .ToArray();
        }
    }
}
