using NUnit.Framework;
using QuickCheck.Internal;
using QuickCheck.NUnit;

namespace QuickCheck.Examples
{
    [TestFixture]
    public class DataProperties
    {
        [Test]
        public void Diff()
        {
            var a = new Position(new Latitude(1, new Variance(2)), new Longitude(3, new Variance(4)));
            var b = new Position(new Latitude(1, new Variance(0)), new Longitude(5, new Variance(4)));

            var da = Reflection.Data(a);
            var db = Reflection.Data(b);

            var diff = da.Diff(db);

            Assert.IsTrue(diff.IsEmpty, diff.ToString());

            Assert.AreEqual(da, db);
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
