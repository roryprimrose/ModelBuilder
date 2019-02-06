namespace ModelBuilder.UnitTests
{
    using System;

    public class DummyValueGenerator : ValueGeneratorBase
    {
        public override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
        {
            return false;
        }

        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }
    }
}