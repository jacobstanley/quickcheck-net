namespace QuickCheck.Random
{
    public class SystemRandom : IRandom
    {
        private readonly System.Random m_Random;

        public SystemRandom(int seed)
        {
            m_Random = new System.Random(seed);
        }

        public uint UInt32()
        {
            return (uint)m_Random.Next();
        }
    }
}
