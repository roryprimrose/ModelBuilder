namespace ModelBuilder.UnitTests
{
    public class Singleton
    {
        private Singleton()
        {
        }

        public static Singleton Instance { get; } = new Singleton();
    }
}