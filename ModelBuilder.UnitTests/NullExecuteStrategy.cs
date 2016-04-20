using System;
using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
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

        public IConstructorResolver ConstructorResolver { get; set; }
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }
        public ICollection<IgnoreRule> IgnoreRules { get; }
        public ICollection<ITypeCreator> TypeCreators { get; }
        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}