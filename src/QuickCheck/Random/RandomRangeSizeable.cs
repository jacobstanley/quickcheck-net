using System;

namespace QuickCheck.Random
{
    public static class RandomRangeSizeable
    {
        public static sbyte Range(this IRandom random, int size, sbyte x1, sbyte x2)
        {
            return (sbyte)random.Range(size, (int)x1, x2);
        }

        public static byte Range(this IRandom random, int size, byte x1, byte x2)
        {
            return (byte)random.Range(size, (int)x1, x2);
        }

        public static short Range(this IRandom random, int size, short x1, short x2)
        {
            return (short)random.Range(size, (int)x1, x2);
        }

        public static ushort Range(this IRandom random, int size, ushort x1, ushort x2)
        {
            return (ushort)random.Range(size, (int)x1, x2);
        }

        public static int Range(this IRandom random, int size, int x1, int x2)
        {
            int min, max;

            if (x1 < x2)
            {
                min = x1;
                max = x2;
            }
            else
            {
                min = x2;
                max = x1;
            }

            int pmin = Bits(min);
            int pmax = Bits(max);
            int p = Math.Max(pmin, Math.Max(pmax, 40));
            int n = Math.Min(p * size / 100, 30);

            int k = 1 << n;
            int kmin = Math.Max(min, -k);
            int kmax = Math.Min(max, k);

            return random.Range(kmin, kmax);
        }

        public static uint Range(this IRandom random, int size, uint x1, uint x2)
        {
            uint min, max;

            if (x1 < x2)
            {
                min = x1;
                max = x2;
            }
            else
            {
                min = x2;
                max = x1;
            }

            int pmin = Bits(min);
            int pmax = Bits(max);
            int p = Math.Max(pmin, Math.Max(pmax, 40));
            int n = Math.Min(p * size / 100, 30);

            uint k = 1U << n;
            uint kmax = Math.Min(max, k);

            return random.Range(min, kmax);
        }

        public static long Range(this IRandom random, int size, long x1, long x2)
        {
            long min, max;

            if (x1 < x2)
            {
                min = x1;
                max = x2;
            }
            else
            {
                min = x2;
                max = x1;
            }

            int pmin = Bits(min);
            int pmax = Bits(max);
            int p = Math.Max(pmin, Math.Max(pmax, 40));
            int n = Math.Min(p * size / 100, 62);

            long k = 1L << n;
            long kmin = Math.Max(min, -k);
            long kmax = Math.Min(max, k);

            return random.Range(kmin, kmax);
        }

        public static ulong Range(this IRandom random, int size, ulong x1, ulong x2)
        {
            ulong min, max;

            if (x1 < x2)
            {
                min = x1;
                max = x2;
            }
            else
            {
                min = x2;
                max = x1;
            }

            int pmin = Bits(min);
            int pmax = Bits(max);
            int p = Math.Max(pmin, Math.Max(pmax, 40));
            int n = Math.Min(p * size / 100, 62);

            ulong k = 1UL << n;
            ulong kmax = Math.Min(max, k);

            return random.Range(min, kmax);
        }

        private static int Bits(long n)
        {
            return Bits((ulong)(n < 0 ? -n : n));
        }

        private static int Bits(ulong n)
        {
            int bits = 0;

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

        public static float Float(this IRandom random, int size, float x1, float x2)
        {
            return x1 + random.Range(0, x2 - x1) * size / 100.0f;
        }

        public static double Double(this IRandom random, int size, double x1, double x2)
        {
            return x1 + random.Range(0, x2 - x1) * size / 100.0;
        }
    }
}
