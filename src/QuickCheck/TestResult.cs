using System;
using QuickCheck.Internal;

namespace QuickCheck
{
    /// <summary>
    /// The result of a single test.
    /// </summary>
    public class TestResult
    {
        private readonly TestArgs m_Args;
        private readonly Exception m_Error;

        private TestResult(TestArgs args, Exception error)
        {
            m_Args = args;
            m_Error = error;
        }

        public TestArgs Args
        {
            get
            {
                return m_Args;
            }
        }

        public Exception Error
        {
            get
            {
                return m_Error;
            }
        }

        public bool IsFailure
        {
            get
            {
                return m_Error != null;
            }
        }

        /// <summary>
        /// Throws if this result is a failure.
        /// </summary>
        public void ThrowError()
        {
            m_Error.Rethrow();
        }

        public static TestResult Success(TestArgs args)
        {
            return new TestResult(args, null);
        }

        public static TestResult Failure(TestArgs args, Exception error)
        {
            return new TestResult(args, error);
        }
    }
}