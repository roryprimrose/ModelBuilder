namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class FactoryWithValue
    {
        private FactoryWithValue(Guid value)
        {
            Value = value;
        }

        public static FactoryWithValue Create(Guid value)
        {
            return new FactoryWithValue(value);
        }

        public Guid Value { get; }
    }
}