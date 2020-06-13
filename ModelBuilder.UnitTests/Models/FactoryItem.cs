namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class FactoryItem
    {
        private FactoryItem()
        {
            Value = Guid.NewGuid();
        }

        public static FactoryItem Create()
        {
            return new FactoryItem();
        }

        public Guid Value { get; }
    }
}