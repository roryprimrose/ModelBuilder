namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class Singleton
    {
        private Singleton()
        {
            Value = Guid.NewGuid().ToString();
        }

        public static Singleton Instance { get; } = new Singleton();

        public string Value { get; }
    }
}