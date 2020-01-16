namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

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

        public Collection<CreationRule> CreationRules { get; }

        public Collection<ExecuteOrderRule> ExecuteOrderRules { get; }

        public Collection<IgnoreRule> IgnoreRules { get; }

        public Collection<IPostBuildAction> PostBuildActions { get; }

        public IPropertyResolver PropertyResolver { get; }

        public Collection<ITypeCreator> TypeCreators { get; }

        public Collection<TypeMappingRule> TypeMappingRules { get; }

        public Collection<IValueGenerator> ValueGenerators { get; }
    }
}