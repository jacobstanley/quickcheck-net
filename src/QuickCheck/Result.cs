using System;
using QuickCheck.Internal;

namespace QuickCheck
{
    /// <summary>
    /// The result of all the tests run on a property.
    /// </summary>
    public class Result
    {
        private readonly int m_Tests;
        private readonly int m_Seed;
        private readonly int m_Size;
        private readonly TestArgs m_Args;
        private readonly Exception m_Error;

        private Result(int tests, int seed, int size, TestArgs args, Exception error)
        {
            m_Tests = tests;
            m_Seed = seed;
            m_Size = size;
            m_Args = args;
            m_Error = error;
        }

        /// <summary>
        /// Throws if this result is a failure.
        /// </summary>
        public void ThrowError()
        {
            string tests = m_Tests > 1 ? m_Tests + " tests" : "1 test";
            string seed = "seed = " + m_Seed + ", size = " + m_Size;
            string message = m_Args + "\nFalsified after " + tests + " (" + seed + ")";

            m_Error.RethrowWith(message);
        }

        public static Result Success(int tests)
        {
            return new Result(tests, 0, 0, null, null);
        }

        public static Result Failure(int tests, int seed, int size, TestArgs args, Exception error)
        {
            return new Result(tests, seed, size, args, error);
        }
    }
}