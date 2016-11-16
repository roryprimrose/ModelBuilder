namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class NullExecuteStrategy : IExecuteStrategy
    {
        public object CreateWith(Type type, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IBuildConfiguration configuration, IBuildLog buildLog)
        {
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public LinkedList<object> BuildChain { get; }

        public IBuildConfiguration Configuration { get; }
        public IBuildLog Log { get; }
    }
}