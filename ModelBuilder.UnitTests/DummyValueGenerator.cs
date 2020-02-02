namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.ValueGenerators;

    public class DummyValueGenerator : ValueGeneratorBase
    {
        protected override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
        {
            return false;
        }

        protected override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }
    }
}