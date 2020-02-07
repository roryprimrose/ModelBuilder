namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.ValueGenerators;

    public class DummyValueGenerator : ValueGeneratorBase
    {
        public override bool IsMatch(Type type, string referenceName, IBuildChain buildChain)
        {
            return false;
        }

        public override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }
    }
}