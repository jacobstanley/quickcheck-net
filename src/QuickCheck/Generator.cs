using System;

namespace QuickCheck
{
    public class Generator
    {
        private readonly Random m_Random;

        public Generator(int seed)
        {
            m_Random = new Random(seed);
        }

        public T Arbitrary<T>(int size)
        {
            return Quick.Generator<T>().Arbitrary(this, size);
        }

        public int Int32()
        {
            return m_Random.Next();
        }

        public bool Bool()
        {
            return Choose(0, 1) != 0;
        }

        public long Int64()
        {
            Int64 high = Int32();
            Int64 low = Int32();
            return high << 32 | low;
        }

        public double Double()
        {
            return m_Random.NextDouble();
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
            return m_Random.Next(min, max);
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
