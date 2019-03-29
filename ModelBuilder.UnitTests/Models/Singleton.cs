namespace ModelBuilder.UnitTests.Models
{
    public class Singleton
    {
        private Singleton()
        {
        }

        public static Singleton Instance { get; } = new Singleton();

        public string Value { get; set; }
    }
}