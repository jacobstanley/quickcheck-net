using System;
using QuickCheck.Internal;

namespace QuickCheck
{
    public class Generator
    {
        private readonly MersenneTwister m_Twister;

        public Generator(int seed)
        {
            m_Twister = new MersenneTwister((uint)seed);
        }

        public T Arbitrary<T>(int size)
        {
            return Quick.Generator<T>().Arbitrary(this, size);
        }

        public bool Bool()
        {
            return Choose(0, 1) != 0;
        }

        public int Int32()
        {
            return (int)UInt32();
        }

        public long Int64()
        {
            return (long)UInt64();
        }

        public uint UInt32()
        {
            return m_Twister.Next();
        }

        public ulong UInt64()
        {
            ulong hi = UInt32();
            ulong lo = UInt32();
            return hi << 32 | lo;
        }

        public double Double()
        {
            // 53-bits of precision
            uint a = UInt32() >> 5;
            uint b = UInt32() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }

        public double Double(int size)
        {
            const long precision = 9999999999999;

            long n = size;
            double a = Choose(-n * precision, n * precision);
            double b = Choose(1, precision);

            // TODO: Convert rational to double without huge loss of precision.
            return a / b;
        }

        public double Double(int size, double max)
        {
            return Choose(0.0, max * size / 100.0);
        }

        public int Choose(int min, int max)
        {
            if (min == max)
            {
                return min;
            }

            long diff = max - min + 1;
            long rand = UInt32();
            return (int)(min + rand % diff);
        }

        public long Choose(long min, long max)
        {
            if (min == max)
            {
                return min;
            }

            // unsound for numbers over 60-bits
            long diff = max - min + 1;
            return min + Int64() % diff;
        }

        public double Choose(double min, double max)
        {
            double diff = max - min;
            return min + Double() * diff;
        }

        public int Choose(int size, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException("max must be greater than min");
            }

            int n = Math.Max(40 * size / 100, 30);
            int k = 1 << n;
            int kmin = Math.Max(min, -k);
            int kmax = Math.Min(max, k);

            return Choose(kmin, kmax);
        }

        public long Choose(int size, long min, long max)
        {
            if (min > max)
            {
                throw new ArgumentException("max must be greater than min");
            }

            int pmin = Bits(min);
            int pmax = Bits(max);
            int p = Math.Max(pmin, Math.Max(pmax, 40));
            int n = Math.Min(p * size / 100, 62);

            long k = 1L << n;
            long kmin = Math.Max(min, -k);
            long kmax = Math.Min(max, k);

            return Choose(kmin, kmax);
        }

        private static int Bits(long x)
        {
            int bits = 0;
            ulong n = (ulong)(x < 0 ? -x : x);

            if (n >> 32 != 0) { bits += 32; n >>= 32; }
            if (n >> 16 != 0) { bits += 16; n >>= 16; }
            if (n >>  8 != 0) { bits +=  8; n >>=  8; }
            if (n >>  4 != 0) { bits +=  4; n >>=  4; }
            if (n >>  2 != 0) { bits +=  2; n >>=  2; }
            if (n >>  1 != 0) { bits +=  1;           }

            // only 0 or 1 left, both require at
            // least 1 bit to be stored
            return bits + 1;
        }
    }
}
