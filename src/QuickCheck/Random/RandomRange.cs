using System;

namespace QuickCheck.Random
{
    // TODO: This class should probably be generated.
    // NOTE: The code for every integral type should be basically the
    // NOTE: same, keep this in mind if you make a bugfix to one of
    // NOTE: the methods. The main difference is that signed types
    // NOTE: need to use unsigned arithmetic for some of their operations.
    public static class RandomRange
    {
        /// <summary>
        /// Generate a uniformly distributed <see cref="SByte"/> in the
        /// range [x1, x2].
        /// </summary>
        public static SByte Range(this IRandom random, SByte x1, SByte x2)
        {
            return (SByte)random.Range((Int32)x1, x2);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="Int16"/> in the
        /// range [x1, x2].
        /// </summary>
        public static Int16 Range(this IRandom random, Int16 x1, Int16 x2)
        {
            return (Int16)random.Range((Int32)x1, x2);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="Int32"/> in the
        /// range [x1, x2].
        /// </summary>
        public static Int32 Range(this IRandom random, Int32 x1, Int32 x2)
        {
            Int32 i, j;

            if (x1 < x2)
            {
                i = x1;
                j = x2;
            }
            else
            {
                i = x2;
                j = x1;
            }

            UInt32 n = 1 + Sub(j, i);

            if (n == 0)
            {
                // j == Int32.MaxValue
                // i == Int32.MinValue
                return random.Int32();
            }

            UInt32 buckets = UInt32.MaxValue / n;
            UInt32 maxN = buckets * n;

            while (true)
            {
                UInt32 x = random.UInt32();

                if (x < maxN)
                {
                    return Add(i, x / buckets);
                }
            }
        }

        private static UInt32 Sub(Int32 x, Int32 y)
        {
            return (UInt32)x - (UInt32)y;
        }

        private static Int32 Add(Int32 x, UInt32 y)
        {
            return x + (Int32)y;
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="Int64"/> in the
        /// range [x1, x2].
        /// </summary>
        public static Int64 Range(this IRandom random, Int64 x1, Int64 x2)
        {
            Int64 i, j;

            if (x1 < x2)
            {
                i = x1;
                j = x2;
            }
            else
            {
                i = x2;
                j = x1;
            }

            UInt64 n = 1 + Sub(j, i);

            if (n == 0)
            {
                // j == Int64.MaxValue
                // i == Int64.MinValue
                return random.Int64();
            }

            UInt64 buckets = UInt64.MaxValue / n;
            UInt64 maxN = buckets * n;

            while (true)
            {
                UInt64 x = random.UInt64();

                if (x < maxN)
                {
                    return Add(i, x / buckets);
                }
            }
        }

        private static UInt64 Sub(Int64 x, Int64 y)
        {
            return (UInt64)x - (UInt64)y;
        }

        private static Int64 Add(Int64 x, UInt64 y)
        {
            return x + (Int64)y;
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="Byte"/> in the
        /// range [x1, x2].
        /// </summary>
        public static Byte Range(this IRandom random, Byte x1, Byte x2)
        {
            return (Byte)random.Range((UInt32)x1, x2);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="UInt16"/> in the
        /// range [x1, x2].
        /// </summary>
        public static UInt16 Range(this IRandom random, UInt16 x1, UInt16 x2)
        {
            return (UInt16)random.Range((UInt32)x1, x2);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="UInt32"/> in the
        /// range [x1, x2].
        /// </summary>
        public static UInt32 Range(this IRandom random, UInt32 x1, UInt32 x2)
        {
            UInt32 i, j;

            if (x1 < x2)
            {
                i = x1;
                j = x2;
            }
            else
            {
                i = x2;
                j = x1;
            }

            UInt32 n = 1 + j - i;

            if (n == 0)
            {
                // j == UInt32.MaxValue
                // i == UInt32.MinValue
                return random.UInt32();
            }

            UInt32 buckets = UInt32.MaxValue / n;
            UInt32 maxN = buckets * n;

            while (true)
            {
                UInt32 x = random.UInt32();

                if (x < maxN)
                {
                    return i + (x / buckets);
                }
            }
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="UInt64"/> in the
        /// range [x1, x2].
        /// </summary>
        public static UInt64 Range(this IRandom random, UInt64 x1, UInt64 x2)
        {
            UInt64 i, j;

            if (x1 < x2)
            {
                i = x1;
                j = x2;
            }
            else
            {
                i = x2;
                j = x1;
            }

            UInt64 n = 1 + j - i;

            if (n == 0)
            {
                // j == UInt64.MaxValue
                // i == UInt64.MinValue
                return random.UInt64();
            }

            UInt64 buckets = UInt64.MaxValue / n;
            UInt64 maxN = buckets * n;

            while (true)
            {
                UInt64 x = random.UInt64();

                if (x < maxN)
                {
                    return i + (x / buckets);
                }
            }
        }

        // NOTE: The types below do not use the same boilerplate as
        // NOTE: above when generating ranges.

        /// <summary>
        /// Generate a uniformly distributed <see cref="bool"/> in the
        /// range [x1, x2].
        /// </summary>
        public static bool Range(this IRandom random, bool x1, bool x2)
        {
            return x1 == x2 ? x1 : random.Bool();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="float"/> in the
        /// range [x1, x2].
        /// </summary>
        public static float Range(this IRandom random, float x1, float x2)
        {
            uint u = random.UInt32();
            return x1 + (x2 - x1) * Random.ToFloat(u);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="double"/> in the
        /// range [x1, x2].
        /// </summary>
        public static double Range(this IRandom random, double x1, double x2)
        {
            uint u = random.UInt32();
            uint v = random.UInt32();
            return x1 + (x2 - x1) * Random.ToDouble(u, v);
        }
    }
}
