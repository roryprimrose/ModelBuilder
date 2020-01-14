namespace ModelBuilder.UnitTests
{
    using System;
    using Models;

    public class DummyPostBuildAction : IPostBuildAction
    {
        public void Execute(Type type, string referenceName, IBuildChain buildChain)
        {
        }

        public bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
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