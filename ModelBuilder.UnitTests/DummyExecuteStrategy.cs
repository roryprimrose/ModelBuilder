namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class DummyExecuteStrategy : IExecuteStrategy<string>
    {
        public string CreateWith(params object[] args)
        {
            throw new NotImplementedException();
        }

        public object CreateWith(Type type, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IBuildConfiguration configuration, IBuildLog buildLog)
        {
        }

        public string Populate(string instance)
        {
            throw new NotImplementedException();
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public LinkedList<object> BuildChain { get; } = new LinkedList<object>();

        public IBuildConfiguration Configuration { get; }
        public IBuildLog Log { get; }
    }
}