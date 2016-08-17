using System;

namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;

    public class DummyValueGenerator : ValueGeneratorBase
    {
        public override bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            return false;
        }

        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            throw new NotImplementedException();
        }
    }
}