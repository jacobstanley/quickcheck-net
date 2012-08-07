namespace QuickCheck.Random
{
    public class MersenneTwisterFactory : IRandomFactory
    {
        private readonly System.Random m_Seed = new System.Random();

        public IRandom NewRandom(out ulong seed)
        {
            seed = (ulong)m_Seed.Next();
            return new MersenneTwisterRandom((uint)seed);
        }

        public IRandom NewRandom(ulong seed)
        {
            return new MersenneTwisterRandom((uint)seed);
        }
    }
}
