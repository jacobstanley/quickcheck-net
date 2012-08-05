using System;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using System.Reflection;

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

        [TestCaseSource("GetProperties")]
        public void Check(TestProperty property)
        {
            try
            {
                property.Check(this);
            }
            catch (TargetInvocationException exception)
            {
                Exception e = exception.InnerException;

                while (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                PreserveStackTrace(e);

// ReSharper disable PossibleNullReferenceException
                FieldInfo remoteStackField = typeof(Exception)
                    .GetField("_remoteStackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
                string remoteStack = (string)remoteStackField.GetValue(e);
                remoteStackField.SetValue(e, remoteStack
                    .Split('\n')
                    .Where(x => !x.StartsWith("   at QuickCheck.Quick"))
                    .Aggregate((ys, y) => ys + "\n" + y));

                try
                {
                    throw e;
                }
                finally
                {
                    typeof(Exception)
                        .GetField("_stackTrace", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(e, null);
                }
// ReSharper restore PossibleNullReferenceException
            }
        }

        private static void PreserveStackTrace(Exception exception)
        {
            var context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            var manager = new ObjectManager(null, context);
            var info = new SerializationInfo(exception.GetType(), new FormatterConverter());

            exception.GetObjectData(info, context);
            manager.RegisterObject(exception, 1, info);
            manager.DoFixups();
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
