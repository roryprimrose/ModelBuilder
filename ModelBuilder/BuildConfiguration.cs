namespace ModelBuilder
{
    using System.Collections.Generic;
    using ModelBuilder.CreationRules;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildConfiguration" />
    ///     class is used to contain all the configuration required to create values.
    /// </summary>
    public class BuildConfiguration : IBuildConfiguration
    {
        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; set; } = new DefaultConstructorResolver(CacheLevel.PerInstance);

        /// <inheritdoc />
        public ICollection<ICreationRule> CreationRules { get; } = new List<ICreationRule>();

        /// <inheritdoc />
        public ICollection<IExecuteOrderRule> ExecuteOrderRules { get; } = new List<IExecuteOrderRule>();

        /// <inheritdoc />
        public ICollection<IIgnoreRule> IgnoreRules { get; } = new List<IIgnoreRule>();

        /// <inheritdoc />
        public ICollection<IPostBuildAction> PostBuildActions { get; } = new List<IPostBuildAction>();

        /// <inheritdoc />
        public IPropertyResolver PropertyResolver { get; set; } = new DefaultPropertyResolver(CacheLevel.PerInstance);

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; } = new List<ITypeCreator>();

        /// <inheritdoc />
        public ICollection<TypeMappingRule> TypeMappingRules { get; } = new List<TypeMappingRule>();

        /// <inheritdoc />
        public ITypeResolver TypeResolver { get; set; } = new DefaultTypeResolver();

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; } = new List<IValueGenerator>();
    }
}