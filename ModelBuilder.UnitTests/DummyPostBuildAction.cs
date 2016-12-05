namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class DummyPostBuildAction : IPostBuildAction
    {
        public void Execute(Type type, string referenceName, LinkedList<object> buildChain)
        {
        }

        public bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            if (type == typeof(Company))
            {
                return true;
            }

            return false;
        }

        public int Priority { get; }
    }
}