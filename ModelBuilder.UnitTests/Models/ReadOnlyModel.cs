namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class ReadOnlyModel
    {
        public ReadOnlyModel(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }
    }
}