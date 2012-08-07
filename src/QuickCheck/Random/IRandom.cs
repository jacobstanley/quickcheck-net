using System;

namespace QuickCheck.Random
{
    public interface IRandom
    {
        /// <summary>
        /// Generate a uniformly distributed <see cref="uint"/> in the
        /// range [0, 2^32)
        /// </summary>
        UInt32 UInt32();
    }
}
