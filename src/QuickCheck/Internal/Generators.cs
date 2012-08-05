using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickCheck.Internal
{
    public class Int32Generator : IGenerator<Int32>
    {
        public int Arbitrary(Generator gen, int size)
        {
            return (int)gen.Choose(size, Int32.MinValue, Int32.MaxValue);
        }
    }

    public class Int64Generator : IGenerator<Int64>
    {
        public long Arbitrary(Generator gen, int size)
        {
            return gen.Choose(size, Int64.MinValue, Int64.MaxValue);
        }
    }

    public class DoubleGenerator : IGenerator<Double>
    {
        public double Arbitrary(Generator gen, int size)
        {
            return gen.Double(size);
        }
    }

    public class EnumerableGenerator<T> : IGenerator<IEnumerable<T>>
    {
        public IEnumerable<T> Arbitrary(Generator gen, int size)
        {
            int n = gen.Choose(0, size - 1);
            for (int i = 0; i < n; i++)
            {
                yield return gen.Arbitrary<T>(size);
            }
        }
    }

    public class ListGenerator<T> : IGenerator<List<T>>
    {
        public List<T> Arbitrary(Generator gen, int size)
        {
            return gen.Arbitrary<IEnumerable<T>>(size).ToList();
        }
    }

    public class ArrayGenerator<T> : IGenerator<T[]>
    {
        public T[] Arbitrary(Generator gen, int size)
        {
            return gen.Arbitrary<IEnumerable<T>>(size).ToArray();
        }
    }

    public class HashSetGenerator<T> : IGenerator<HashSet<T>>
    {
        public HashSet<T> Arbitrary(Generator gen, int size)
        {
            return new HashSet<T>(gen.Arbitrary<IEnumerable<T>>(size));
        }
    }
}
