namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class DummyPostBuildAction : IPostBuildAction
    {
        public void Execute(Type type, string referenceName, LinkedList<object> buildChain)
        {
            throw new NotImplementedException();
        }

        public bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            return false;
        }

        public int Priority { get; }
    }
}