using System;

namespace ModelBuilder.UnitTests
{
    public class DummyValueGenerator : ValueGeneratorBase
    {
        public override object Generate(Type type, string referenceName, object context)
        {
            throw new NotImplementedException();
        }

        public override bool IsSupported(Type type, string referenceName, object context)
        {
            throw new NotImplementedException();
        }
    }
}