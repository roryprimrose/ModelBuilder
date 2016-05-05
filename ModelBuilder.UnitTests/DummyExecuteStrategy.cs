using System;
using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
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

        public IBuildStrategy BuildStrategy { get; set; }
    }
}