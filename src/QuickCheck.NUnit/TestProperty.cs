using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace QuickCheck.NUnit
{
    public class TestProperty : ITestCaseData
    {
        private readonly MethodInfo m_Method;
        private readonly string m_TestName;

        public TestProperty(MethodInfo method)
        {
            m_Method = method;
            m_TestName = ToSentence(method.Name);
        }

        public void Check(object fixture)
        {
            Type[] args = m_Method
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            Type actionType = Expression.GetActionType(args);
            Delegate action = Delegate.CreateDelegate(actionType, fixture, m_Method);

            MethodInfo quickCheck = CheckMethod(actionType);
            
            quickCheck.Invoke(null, new object[] { action });
        }

        private static string ToSentence(string name)
        {
            if (name.EndsWith("_"))
            {
                name = name.Substring(0, name.Length - 1) + ".";
            }

            return " " + Char.ToUpper(name[0]) + name.Substring(1).Replace('_', ' ');
        }

        public string TestName
        {
            get
            {
                return m_TestName;
            }
        }

        public object[] Arguments
        {
            get
            {
                return new object[] { this };
            }
        }

        public string Description
        {
            get
            {
                return null;
            }
        }

        public Type ExpectedException
        {
            get
            {
                return null;
            }
        }

        public string ExpectedExceptionName
        {
            get
            {
                return null;
            }
        }

        public bool Explicit
        {
            get
            {
                return false;
            }
        }

        public bool HasExpectedResult
        {
            get
            {
                return false;
            }
        }

        public string IgnoreReason
        {
            get
            {
                return null;
            }
        }

        public bool Ignored
        {
            get
            {
                return false;
            }
        }

        public object Result
        {
            get
            {
                return null;
            }
        }

        private static readonly Dictionary<Type, MethodInfo> s_CheckMethods = CreateLookup();

        private static Dictionary<Type, MethodInfo> CreateLookup()
        {
            var lookup = new Dictionary<Type, MethodInfo>();
            var methods = typeof(Quick)
                .GetMethods()
                .Where(x => x.Name == "Check");

            foreach (var method in methods)
            {
                var ps = method.GetParameters();

                if (ps.Length != 1)
                {
                    continue;
                }

                var type = ps[0].ParameterType;

                if (method.IsGenericMethod)
                {
                    lookup.Add(type.GetGenericTypeDefinition(), method);
                }
                else
                {
                    lookup.Add(type, method);
                }
            }

            return lookup;
        }

        private static MethodInfo CheckMethod(Type arg)
        {
            MethodInfo method;

            if (!arg.IsGenericType)
            {
                s_CheckMethods.TryGetValue(arg, out method);
                return method;
            }

            if (s_CheckMethods.TryGetValue(arg.GetGenericTypeDefinition(), out method))
            {
                return method.MakeGenericMethod(arg.GetGenericArguments());
            }

            throw new NotSupportedException("Could not find Quick.Check(" + arg.Name + ")");
        }
    }
}
