using System;
using System.Diagnostics;

namespace QuickCheck.Random
{
    /// <summary>
    /// A simple random number generator based on George Marsaglia's MWC (multiply
    /// with carry) generator.
    /// </summary>
    /// <remarks>
    /// Originally written by John D. Cook 
    /// http://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
    /// http://www.johndcook.com
    /// </remarks>
    public class MwcRandom : IRandom
    {
        // These values are not magical, just the default values Marsaglia used.
        // Any pair of unsigned integers should be fine.
        private uint m_Seed1 = 521288629;
        private uint m_Seed2 = 362436069;

        /// <summary>
        /// The current seed.
        /// </summary>
        public ulong Seed
        {
            get
            {
                return m_Seed1 | m_Seed2 << 32;
            }
        }

        public MwcRandom()
            : this((uint)Stopwatch.GetTimestamp(), (uint)Environment.TickCount)
        {
        }

        public MwcRandom(ulong seed)
            : this((uint)seed, (uint)(seed >> 32))
        {
        }

        public MwcRandom(uint seed1, uint seed2)
        {
            if (seed1 != 0) m_Seed1 = seed1;
            if (seed2 != 0) m_Seed2 = seed2;
        }

        public uint UInt32()
        {
            // This is the heart of the generator.
            // It uses George Marsaglia's MWC algorithm to produce an unsigned integer.
            // See http://www.bobwheeler.com/statistics/Password/MarsagliaPost.txt

            m_Seed1 = 18000 * (m_Seed1 & 65535) + (m_Seed1 >> 16);
            m_Seed2 = 36969 * (m_Seed2 & 65535) + (m_Seed2 >> 16);

            return m_Seed1 + (m_Seed2 << 16);
        }
    }
}
