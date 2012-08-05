using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickCheck
{
    public class Generator
    {
        private readonly int m_Seed;
        private readonly Random m_Random;

        public Generator(int seed)
        {
            m_Seed = seed;
            m_Random = new Random(seed);
        }

        public int Seed
        {
            get
            {
                return m_Seed;
            }
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

        public int Choose(int min, int max)
        {
            return (int)Choose((long)min, max);
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

        public double Choose(double min, double max)
        {
            double diff = max - min;
            return min + Double() * diff;
        }

        private static int Bits(long n)
        {
            if (n == 0)
            {
                return 0;
            }

            return 1 + Bits(n / 2);
        }
    }
}
