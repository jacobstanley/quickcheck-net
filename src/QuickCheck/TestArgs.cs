using System;
using System.Reflection;
using System.Text;
using QuickCheck.Internal;

namespace QuickCheck
{
    public class TestArgs
    {
        private readonly MethodInfo m_Method;
        private readonly object[] m_Args;

        public TestArgs(MethodInfo method, params object[] args)
        {
            m_Method = method;
            m_Args = args;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            ParameterInfo[] ps = m_Method.GetParameters();
            int n = Math.Max(ps.Length, m_Args.Length);

            for (int i = 0; i < n; i++)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                if (i < ps.Length)
                {
                    sb.Append(ps[i].Name);
                    sb.Append(" = ");
                }

                if (i < m_Args.Length)
                {
                    sb.Append(Reflection.Show(m_Args[i]));
                }
                else
                {
                    // should never happen
                    sb.Append("?");
                }
            }

            return sb.ToString();
        }
    }
}