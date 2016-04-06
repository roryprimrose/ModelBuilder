using System;

namespace ModelBuilder.UnitTests
{
    public class DummyTypeCreator : TypeCreatorBase
    {
        public override object Create(Type type, string referenceName, object context, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void VerifyWithNullType()
        {
            VerifyCreateRequest(null, null, null);
        }
    }
}