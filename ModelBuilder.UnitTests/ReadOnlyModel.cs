using System;

namespace ModelBuilder.UnitTests
{
    public class ReadOnlyModel
    {
        public ReadOnlyModel(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; private set; }
    }
}