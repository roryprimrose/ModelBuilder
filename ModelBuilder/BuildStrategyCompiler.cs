namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.Properties;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildStrategyCompiler" />
    ///     class is used to assist in creating a new <see cref="IBuildStrategy" /> instance.
    /// </summary>
    public class BuildStrategyCompiler : IBuildStrategyCompiler
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildStrategyCompiler" /> class.
        /// </summary>
        public BuildStrategyCompiler()
        {
            CreationRules = new List<CreationRule>();
            ExecuteOrderRules = new List<ExecuteOrderRule>();
            IgnoreRules = new List<IgnoreRule>();
            PostBuildActions = new List<IPostBuildAction>();
            TypeCreators = new List<ITypeCreator>();
            ValueGenerators = new List<IValueGenerator>();
            TypeMappingRules = new List<TypeMappingRule>();
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">The <see cref="ConstructorResolver" /> property is <c>null</c>.</exception>
        public IBuildStrategy Compile()
        {
            if (ConstructorResolver == null)
            {
                throw new InvalidOperationException(Resources.BuildStrategyCompiler_NullConstructorResolver);
            }

            if (PropertyResolver == null)
            {
                throw new InvalidOperationException(Resources.BuildStrategyCompiler_NullPropertyResolver);
            }

            return new BuildStrategy(
                ConstructorResolver,
                PropertyResolver,
                CreationRules,
                TypeCreators,
                ValueGenerators,
                IgnoreRules,
                TypeMappingRules,
                ExecuteOrderRules,
                PostBuildActions);
        }

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; set; }

        /// <inheritdoc />
        public ICollection<CreationRule> CreationRules { get; }

        /// <inheritdoc />
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <inheritdoc />
        public ICollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public ICollection<IPostBuildAction> PostBuildActions { get; }

        /// <inheritdoc />
        public IPropertyResolver PropertyResolver { get; set; }

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public ICollection<TypeMappingRule> TypeMappingRules { get; }

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}