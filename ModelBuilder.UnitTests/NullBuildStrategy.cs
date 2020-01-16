namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    public class NullBuildStrategy : IBuildStrategy
    {
        public IBuildLog GetBuildLog()
        {
            throw new NotImplementedException();
        }

        public IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            throw new NotImplementedException();
        }

        public IConstructorResolver ConstructorResolver { get; set; }

        public ICollection<CreationRule> CreationRules { get; }

        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        public ICollection<IgnoreRule> IgnoreRules { get; }

        public ICollection<IPostBuildAction> PostBuildActions { get; }

        public IPropertyResolver PropertyResolver { get; set; }

        public ICollection<ITypeCreator> TypeCreators { get; }

        public ICollection<TypeMappingRule> TypeMappingRules { get; }

        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}