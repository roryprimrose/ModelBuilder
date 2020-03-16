namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.TypeCreators;

    public class DummyTypeCreator : TypeCreatorBase
    {
        protected override bool CanCreate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string referenceName)
        {
            return false;
        }

        protected override bool CanPopulate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string referenceName)
        {
            return false;
        }

        protected override object CreateInstance(IExecuteStrategy executeStrategy,
            Type type,
            string referenceName,
            params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotImplementedException();
        }
    }
}