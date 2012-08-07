using System;
using System.Collections.Generic;
using System.Linq;
using QuickCheck.Random;

namespace QuickCheck.Internal
{
    public class SByteGenerator : IGenerator<SByte>
    {
        public SByte Arbitrary(IRandom gen, int size)
        {
            return gen.SByte(size);
        }
    }

    public class Int16Generator : IGenerator<Int16>
    {
        public Int16 Arbitrary(IRandom gen, int size)
        {
            return gen.Int16(size);
        }
    }

    public class Int32Generator : IGenerator<Int32>
    {
        public Int32 Arbitrary(IRandom gen, int size)
        {
            return gen.Int32(size);
        }
    }

    public class Int64Generator : IGenerator<Int64>
    {
        public Int64 Arbitrary(IRandom gen, int size)
        {
            return gen.Int64(size);
        }
    }

    public class ByteGenerator : IGenerator<Byte>
    {
        public Byte Arbitrary(IRandom gen, int size)
        {
            return gen.Byte(size);
        }
    }

    public class UInt16Generator : IGenerator<UInt16>
    {
        public UInt16 Arbitrary(IRandom gen, int size)
        {
            return gen.UInt16(size);
        }
    }

    public class UInt32Generator : IGenerator<UInt32>
    {
        public UInt32 Arbitrary(IRandom gen, int size)
        {
            return gen.UInt32(size);
        }
    }

    public class UInt64Generator : IGenerator<UInt64>
    {
        public UInt64 Arbitrary(IRandom gen, int size)
        {
            return gen.UInt64(size);
        }
    }

    public class DoubleGenerator : IGenerator<Double>
    {
        public double Arbitrary(IRandom gen, int size)
        {
            return gen.Double(size);
        }
    }

    public class FloatGenerator : IGenerator<Single>
    {
        public float Arbitrary(IRandom gen, int size)
        {
            return (float)gen.Double(size);
        }
    }

    public class EnumerableGenerator<T> : IGenerator<IEnumerable<T>>
    {
        public IEnumerable<T> Arbitrary(IRandom gen, int size)
        {
            int n = gen.Range(0, size - 1);
            for (int i = 0; i < n; i++)
            {
                yield return gen.Arbitrary<T>(size);
            }
        }
    }

    public class ListGenerator<T> : IGenerator<List<T>>
    {
        public List<T> Arbitrary(IRandom gen, int size)
        {
            return gen.Arbitrary<IEnumerable<T>>(size).ToList();
        }
    }

    public class ArrayGenerator<T> : IGenerator<T[]>
    {
        public T[] Arbitrary(IRandom gen, int size)
        {
            return gen.Arbitrary<IEnumerable<T>>(size).ToArray();
        }
    }

    public class HashSetGenerator<T> : IGenerator<HashSet<T>>
    {
        public HashSet<T> Arbitrary(IRandom gen, int size)
        {
            return new HashSet<T>(gen.Arbitrary<IEnumerable<T>>(size));
        }
    }
}
