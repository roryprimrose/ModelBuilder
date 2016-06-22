namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;

    public class NullBuildStrategy : IBuildStrategy
    {
        public IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            throw new NotImplementedException();
        }

        public IBuildLog BuildLog { get; }

        public IConstructorResolver ConstructorResolver { get; }

        public ReadOnlyCollection<CreationRule> CreationRules { get; }

        public ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        public ReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        public ReadOnlyCollection<IPostBuildAction> PostBuildActions { get; }

        public ReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        public ReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}