using System;
using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
    public class DummyBuildStrategy : IBuildStrategy
    {
        public IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            throw new NotImplementedException();
        }

        public IConstructorResolver ConstructorResolver { get; }
        public IReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }
        public IReadOnlyCollection<IgnoreRule> IgnoreRules { get; }
        public IReadOnlyCollection<ITypeCreator> TypeCreators { get; }
        public IReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}