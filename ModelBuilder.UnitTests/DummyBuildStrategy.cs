namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;

    public class DummyBuildStrategy : IBuildStrategy
    {
        public IBuildLog GetBuildLog()
        {
            throw new NotImplementedException();
        }

        public IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            throw new NotImplementedException();
        }

        public IConstructorResolver ConstructorResolver { get; }

        public ReadOnlyCollection<CreationRule> CreationRules { get; }

        public ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        public ReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        public ReadOnlyCollection<IPostBuildAction> PostBuildActions { get; }

        public IPropertyResolver PropertyResolver { get; }

        public ReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        public ReadOnlyCollection<TypeMappingRule> TypeMappingRules { get; }

        public ReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}