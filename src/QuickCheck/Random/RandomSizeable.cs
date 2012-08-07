namespace QuickCheck.Random
{
    public static class RandomSizeable
    {
        public static sbyte SByte(this IRandom random, int size)
        {
            return random.Range(size, sbyte.MinValue, sbyte.MaxValue);
        }

        public static short Int16(this IRandom random, int size)
        {
            return random.Range(size, short.MinValue, short.MaxValue);
        }

        public static int Int32(this IRandom random, int size)
        {
            return random.Range(size, int.MinValue, int.MaxValue);
        }

        public static long Int64(this IRandom random, int size)
        {
            return random.Range(size, long.MinValue, long.MaxValue);
        }

        public static byte Byte(this IRandom random, int size)
        {
            return random.Range(size, byte.MinValue, byte.MaxValue);
        }

        public static ushort UInt16(this IRandom random, int size)
        {
            return random.Range(size, ushort.MinValue, ushort.MaxValue);
        }

        public static uint UInt32(this IRandom random, int size)
        {
            return random.Range(size, uint.MinValue, uint.MaxValue);
        }

        public static ulong UInt64(this IRandom random, int size)
        {
            return random.Range(size, ulong.MinValue, ulong.MaxValue);
        }

        public static float Float(this IRandom random, int size)
        {
            const long precision = 9999999;

            long n = size;
            float a = random.Range(-n * precision, n * precision);
            float b = random.Range(1, precision);

            // TODO: Convert rational to double without huge loss of precision.
            return a / b;
        }

        public static double Double(this IRandom random, int size)
        {
            const long precision = 9999999999999;

            long n = size;
            double a = random.Range(-n * precision, n * precision);
            double b = random.Range(1, precision);

            // TODO: Convert rational to double without huge loss of precision.
            return a / b;
        }
    }
}
