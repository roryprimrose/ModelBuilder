namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.ValueGenerators;

    public class DummyValueGenerator : ValueGeneratorBase
    {
        protected override bool IsMatch(IBuildChain buildChain, Type type, string referenceName)
        {
            return false;
        }

        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            throw new NotImplementedException();
        }
    }
}