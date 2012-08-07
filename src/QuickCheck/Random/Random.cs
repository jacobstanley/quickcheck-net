using System;

namespace QuickCheck.Random
{
    public static class Random
    {
        /// <summary>
        /// Generate a uniformly distributed <see cref="sbyte"/> in the
        /// range [-2^4, 2^4).
        /// </summary>
        public static SByte SByte(this IRandom random)
        {
            return (SByte)random.UInt32();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="short"/> in the
        /// range [-2^8, 2^8).
        /// </summary>
        public static Int16 Int16(this IRandom random)
        {
            return (Int16)random.UInt32();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="int"/> in the
        /// range [-2^16, 2^16).
        /// </summary>
        public static Int32 Int32(this IRandom random)
        {
            return (Int32)random.UInt32();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="long"/> in the
        /// range [-2^32, 2^32).
        /// </summary>
        public static Int64 Int64(this IRandom random)
        {
            return (Int64)random.UInt64();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="byte"/> in the
        /// range [0, 2^8).
        /// </summary>
        public static Byte Byte(this IRandom random)
        {
            return (Byte)random.UInt32();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="uint"/> in the
        /// range [0, 2^16).
        /// </summary>
        public static UInt16 UInt16(this IRandom random)
        {
            return (UInt16)random.UInt32();
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="ulong"/> in the
        /// range [0, 2^64).
        /// </summary>
        public static UInt64 UInt64(this IRandom random)
        {
            UInt64 hi = random.UInt32();
            UInt64 lo = random.UInt32();
            return hi << 32 | lo;
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="bool"/> in the
        /// range [false, true].
        /// </summary>
        public static bool Bool(this IRandom random)
        {
            return (random.UInt32() & 1) != 0;
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="float"/> in the
        /// range (0, 1]. Zero is explicitly excluded, to allow generated
        /// values to be used in statistical calculations that require
        /// non-zero values (e.g. uses of the 'log' function).<br/>
        /// <br/>
        /// Subtract Math.Pow(2, -33) to generate a <see cref="float"/>
        /// in a range of [0,1).
        /// </summary>
        public static float Float(this IRandom random)
        {
            uint u = random.UInt32();
            return ToFloat(u);
        }

        /// <summary>
        /// Generate a uniformly distributed <see cref="double"/> in the
        /// range (0, 1]. Zero is explicitly excluded, to allow generated
        /// values to be used in statistical calculations that require
        /// non-zero values (e.g. uses of the 'log' function).<br/>
        /// <br/>
        /// Subtract Math.Pow(2, -53) to generate a <see cref="double"/>
        /// in a range of [0,1).
        /// </summary>
        public static double Double(this IRandom random)
        {
            uint u = random.UInt32();
            uint v = random.UInt32();
            return ToDouble(u, v);
        }

        internal static float ToFloat(uint i)
        {
            // Algorithm taken from Brian O'Sullivan's mwc-random Haskell library.
            const float inv32 = 2.3283064365386962890625e-10f; // 1 / 2^32
            const float inv33 = 1.16415321826934814453125e-10f; // 1 / 2^33

            return i * inv32 + 0.5f + inv33;
        }

        internal static double ToDouble(uint v, uint u)
        {
            // Algorithm taken from Brian O'Sullivan's mwc-random Haskell library.
            const double inv32 = 2.3283064365386962890625e-10; // 1 / 2^32
            const double inv52 = 2.220446049250313080847263336181640625e-16; // 1 / 2^52
            const double inv53 = 1.1102230246251565404236316680908203125e-16; // 1 / 2^53

            return u * inv32 + (0.5 + inv53) + (v & 0xfffff) * inv52;
        }
    }
}
