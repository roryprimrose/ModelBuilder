namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

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

        public void VerifyCreateRequestWithNullType()
        {
            VerifyCreateRequest(null, null, null);
        }

        public void VerifyPopulateRequestWithNullType()
        {
            VerifyPopulateRequest(null, null, null);
        }

        protected override object CreateInstance(Type type, string referenceName, LinkedList<object> buildChain,
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