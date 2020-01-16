namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildStrategyBase" />
    ///     class is used to provide the base framework for a build strategy.
    /// </summary>
    public abstract class BuildStrategyBase : IBuildStrategy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildStrategyBase" /> class.
        /// </summary>
        /// <param name="strategy">The build strategy.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="strategy" /> parameter is null.</exception>
        protected BuildStrategyBase(IBuildStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            ConstructorResolver = strategy.ConstructorResolver;
            PropertyResolver = strategy.PropertyResolver;
            TypeCreators = CopyItems(strategy.TypeCreators);
            ValueGenerators = CopyItems(strategy.ValueGenerators);
            IgnoreRules = CopyItems(strategy.IgnoreRules);
            TypeMappingRules = CopyItems(strategy.TypeMappingRules);
            ExecuteOrderRules = CopyItems(strategy.ExecuteOrderRules);
            PostBuildActions = CopyItems(strategy.PostBuildActions);
            CreationRules = CopyItems(strategy.CreationRules);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildStrategyBase" /> class.
        /// </summary>
        /// <param name="constructorResolver">The constructor resolver.</param>
        /// <param name="propertyResolver">The property resolver.</param>
        /// <param name="creationRules">The creation rules.</param>
        /// <param name="typeCreators">The type creators.</param>
        /// <param name="valueGenerators">The value generators.</param>
        /// <param name="ignoreRules">The ignore rules.</param>
        /// <param name="typeMappingRules">The type mapping rules.</param>
        /// <param name="executeOrderRules">The execute order rules.</param>
        /// <param name="postBuildActions">The post build actions.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorResolver" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyResolver" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creationRules" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreators" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerators" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="ignoreRules" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeMappingRules" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeOrderRules" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildActions" /> parameter is null.</exception>
        protected BuildStrategyBase(
            IConstructorResolver constructorResolver,
            IPropertyResolver propertyResolver,
            IEnumerable<CreationRule> creationRules,
            IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators,
            IEnumerable<IgnoreRule> ignoreRules,
            IEnumerable<TypeMappingRule> typeMappingRules,
            IEnumerable<ExecuteOrderRule> executeOrderRules,
            IEnumerable<IPostBuildAction> postBuildActions)
        {
            if (creationRules == null)
            {
                throw new ArgumentNullException(nameof(creationRules));
            }

            if (typeCreators == null)
            {
                throw new ArgumentNullException(nameof(typeCreators));
            }

            if (constructorResolver == null)
            {
                throw new ArgumentNullException(nameof(constructorResolver));
            }

            if (propertyResolver == null)
            {
                throw new ArgumentNullException(nameof(propertyResolver));
            }

            if (valueGenerators == null)
            {
                throw new ArgumentNullException(nameof(valueGenerators));
            }

            if (ignoreRules == null)
            {
                throw new ArgumentNullException(nameof(ignoreRules));
            }

            if (typeMappingRules == null)
            {
                throw new ArgumentNullException(nameof(typeMappingRules));
            }

            if (executeOrderRules == null)
            {
                throw new ArgumentNullException(nameof(executeOrderRules));
            }

            if (postBuildActions == null)
            {
                throw new ArgumentNullException(nameof(postBuildActions));
            }

            ConstructorResolver = constructorResolver;
            PropertyResolver = propertyResolver;
            TypeCreators = CopyItems(typeCreators);
            ValueGenerators = CopyItems(valueGenerators);
            IgnoreRules = CopyItems(ignoreRules);
            ExecuteOrderRules = CopyItems(executeOrderRules);
            PostBuildActions = CopyItems(postBuildActions);
            CreationRules = CopyItems(creationRules);
            TypeMappingRules = CopyItems(typeMappingRules);
        }

        /// <inheritdoc />
        public abstract IBuildLog GetBuildLog();

        /// <inheritdoc />
        public abstract IExecuteStrategy<T> GetExecuteStrategy<T>();

        private static Collection<T> CopyItems<T>(IEnumerable<T> source)
        {
            return new Collection<T>(source.ToList());
        }

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; }

        /// <inheritdoc />
        public ICollection<CreationRule> CreationRules { get; }

        /// <inheritdoc />
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <inheritdoc />
        public ICollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public ICollection<IPostBuildAction> PostBuildActions { get; }

        /// <inheritdoc />
        public IPropertyResolver PropertyResolver { get; }

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public ICollection<TypeMappingRule> TypeMappingRules { get; }

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}