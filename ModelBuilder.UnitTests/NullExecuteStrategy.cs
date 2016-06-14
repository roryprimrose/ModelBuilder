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

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public LinkedList<object> BuildChain
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