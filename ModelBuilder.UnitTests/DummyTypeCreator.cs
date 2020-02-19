namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.TypeCreators;

    public class DummyTypeCreator : TypeCreatorBase
    {
        public override bool CanCreate(Type type, string referenceName, IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            return false;
        }

        public override bool CanPopulate(Type type, string referenceName, IBuildChain buildChain)
        {
            return false;
        }

        protected override object CreateInstance(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }
    }
}