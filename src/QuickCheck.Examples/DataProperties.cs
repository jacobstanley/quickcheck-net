using QuickCheck.NUnit;
using QuickCheck.Random;

namespace QuickCheck.Examples
{
    public class PositionProperties : TestProperties
    {
        static PositionProperties()
        {
            Quick.Register(typeof(PositionProperties).Assembly);
        }

        public void compare_random_positions(Position a)
        {
            var b = a;

            if (a.Latitude.Value > 85)
            {
                b = new Position(new Latitude(a.Latitude.Value + 1, a.Latitude.Variance), a.Longitude);
            }

            Diff(a, b).WithEpsilon(0.001).AssertEmpty();
        }
    }

    public class PosGen : IGenerator<Position>
    {
        public Position Arbitrary(IRandom gen, int size)
        {
            return new Position(gen.Arbitrary<Latitude>(size), gen.Arbitrary<Longitude>(size));
        }
    }

    public class LatGen : IGenerator<Latitude>
    {
        public Latitude Arbitrary(IRandom gen, int size)
        {
            return new Latitude(gen.Double(size, -90, 90), gen.Arbitrary<Variance>(size));
        }
    }

    public class LonGen : IGenerator<Longitude>
    {
        public Longitude Arbitrary(IRandom gen, int size)
        {
            return new Longitude(gen.Double(size, -180, 180), gen.Arbitrary<Variance>(size));
        }
    }

    public class VarGen : IGenerator<Variance>
    {
        public Variance Arbitrary(IRandom gen, int size)
        {
            return new Variance(gen.Double(size, -10, 10));
        }
    }

    public class Position
    {
        public readonly Latitude Latitude;
        public readonly Longitude Longitude;

        public Position(Latitude latitude, Longitude longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public struct Latitude
    {
        public readonly double Value;
        public readonly Variance Variance;

        public Latitude(double value, Variance variance)
        {
            Value = value;
            Variance = variance;
        }
    }

    public struct Longitude
    {
        public readonly double Value;
        public readonly Variance Variance;

        public Longitude(double value, Variance variance)
        {
            Value = value;
            Variance = variance;
        }
    }

    public struct Variance
    {
        public readonly double Value;

        public Variance(double value)
        {
            Value = value;
        }
    }
}
