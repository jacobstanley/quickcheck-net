namespace QuickCheck.Random
{
    public class SystemRandomFactory : IRandomFactory
    {
        private readonly System.Random m_Seed = new System.Random();

        public IRandom NewRandom(out ulong seed)
        {
            seed = (ulong)m_Seed.Next();
            return new SystemRandom((int)seed);
        }

        public IRandom NewRandom(ulong seed)
        {
            return new SystemRandom((int)seed);
        }
    }
}
