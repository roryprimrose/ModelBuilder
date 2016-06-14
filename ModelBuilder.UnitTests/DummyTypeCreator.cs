namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class DummyTypeCreator : TypeCreatorBase
    {
        public override object Create(
            Type type,
            string referenceName,
            LinkedList<object> buildChain,
            params object[] args)
        {
            throw new NotImplementedException();
        }

        public void VerifyWithNullType()
        {
            VerifyCreateRequest(null, null, null);
        }
    }
}