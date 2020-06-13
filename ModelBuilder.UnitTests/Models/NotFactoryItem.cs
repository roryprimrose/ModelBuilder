namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class NotFactoryItem
    {
        private NotFactoryItem()
        {
            Value = Guid.NewGuid();
        }

        public static NotFactoryItem Create(NotFactoryItem item)
        {
            return item;
        }

        public Guid Value { get; }
    }
}