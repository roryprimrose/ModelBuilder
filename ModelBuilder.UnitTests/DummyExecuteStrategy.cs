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

        public string Populate(string instance)
        {
            throw new NotImplementedException();
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> BuildChain
        {
            get;
        }

        public IBuildStrategy BuildStrategy
        {
            get;
            set;
        }
    }
}