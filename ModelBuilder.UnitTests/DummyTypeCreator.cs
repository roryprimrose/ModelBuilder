namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;

    public class DummyTypeCreator : TypeCreatorBase
    {
        public override bool CanCreate(Type type, string referenceName, LinkedList<object> buildChain)
        {
            return false;
        }

        public override bool CanPopulate(Type type, string referenceName, LinkedList<object> buildChain)
        {
            return false;
        }

        public void VerifyCreateRequestWithNullExecuteStrategy()
        {
            VerifyCreateRequest(typeof(Company), null, null);
        }

        public void VerifyCreateRequestWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            VerifyCreateRequest(null, null, executeStrategy);
        }

        public void VerifyPopulateRequestWithNullExecuteStrategy()
        {
            VerifyPopulateRequest(typeof(Company), null, null);
        }

        public void VerifyPopulateRequestWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            VerifyPopulateRequest(null, null, executeStrategy);
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