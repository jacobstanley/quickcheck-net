namespace QuickCheck.Random
{
    public interface IRandomFactory
    {
        /// <summary>
        /// Create a new random number generator and pass back the seed
        /// used to initialize it.
        /// </summary>
        IRandom NewRandom(out ulong seed);

        /// <summary>
        /// Create a new random number generator with the specified seed.
        /// </summary>
        IRandom NewRandom(ulong seed);
    }
}
