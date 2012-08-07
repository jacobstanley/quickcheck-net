namespace QuickCheck.Random
{
    public class MwcFactory : IRandomFactory
    {
        public IRandom NewRandom(out ulong seed)
        {
            MwcRandom random = new MwcRandom();
            seed = random.Seed;
            return random;
        }

        public IRandom NewRandom(ulong seed)
        {
            return new MwcRandom(seed);
        }
    }
}
