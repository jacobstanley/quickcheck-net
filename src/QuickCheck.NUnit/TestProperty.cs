using System;
using System.Reflection;
using NUnit.Framework;
using QuickCheck.Internal;

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

        public Result Check(object fixture)
        {
            ITestable testable = Reflection.Testable(fixture, m_Method);
            return Quick.Test(testable);
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
    }
}
