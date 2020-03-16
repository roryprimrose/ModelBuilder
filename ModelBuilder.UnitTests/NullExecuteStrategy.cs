namespace ModelBuilder.UnitTests
{
    using System;

    public class NullExecuteStrategy : IExecuteStrategy
    {
        public object Create(Type type, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IBuildConfiguration configuration)
        {
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public IBuildChain BuildChain { get; }

        public IBuildConfiguration Configuration { get; }

        public IBuildLog Log { get; }
    }
}