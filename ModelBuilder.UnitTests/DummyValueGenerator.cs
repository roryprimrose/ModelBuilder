using System;

namespace ModelBuilder.UnitTests
{
    public class DummyValueGenerator : ValueGeneratorBase
    {
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            throw new NotImplementedException();
        }

        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            throw new NotImplementedException();
        }
    }
}