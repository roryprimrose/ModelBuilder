namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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

        public ReadOnlyCollection<CreationRule> CreationRules
        {
            get;
        }

        public ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules
        {
            get;
        }

        public ReadOnlyCollection<IgnoreRule> IgnoreRules
        {
            get;
        }

        public ReadOnlyCollection<ITypeCreator> TypeCreators
        {
            get;
        }

        public ReadOnlyCollection<IValueGenerator> ValueGenerators
        {
            get;
        }
    }
}