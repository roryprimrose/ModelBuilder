namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;

    public class DummyBuildStrategy : IBuildStrategy
    {
        public IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            throw new NotImplementedException();
        }

        public IBuildLog BuildLog
        {
            get;
        }

        public IConstructorResolver ConstructorResolver
        {
            get;
        }

        public IReadOnlyCollection<CreationRule> CreationRules
        {
            get;
        }

        public IReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules
        {
            get;
        }

        public IReadOnlyCollection<IgnoreRule> IgnoreRules
        {
            get;
        }

        public IReadOnlyCollection<ITypeCreator> TypeCreators
        {
            get;
        }

        public IReadOnlyCollection<IValueGenerator> ValueGenerators
        {
            get;
        }
    }
}