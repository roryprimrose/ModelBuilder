namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

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
            TypeCreators = new ReadOnlyCollection<ITypeCreator>(strategy.TypeCreators.ToList());
            ValueGenerators = new ReadOnlyCollection<IValueGenerator>(strategy.ValueGenerators.ToList());
            IgnoreRules = new ReadOnlyCollection<IgnoreRule>(strategy.IgnoreRules.ToList());
            TypeMappingRules = new ReadOnlyCollection<TypeMappingRule>(strategy.TypeMappingRules.ToList());
            ExecuteOrderRules = new ReadOnlyCollection<ExecuteOrderRule>(strategy.ExecuteOrderRules.ToList());
            PostBuildActions = new ReadOnlyCollection<IPostBuildAction>(strategy.PostBuildActions.ToList());
            CreationRules = new ReadOnlyCollection<CreationRule>(strategy.CreationRules.ToList());
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
            TypeCreators = new ReadOnlyCollection<ITypeCreator>(typeCreators.ToList());
            ValueGenerators = new ReadOnlyCollection<IValueGenerator>(valueGenerators.ToList());
            IgnoreRules = new ReadOnlyCollection<IgnoreRule>(ignoreRules.ToList());
            ExecuteOrderRules = new ReadOnlyCollection<ExecuteOrderRule>(executeOrderRules.ToList());
            PostBuildActions = new ReadOnlyCollection<IPostBuildAction>(postBuildActions.ToList());
            CreationRules = new ReadOnlyCollection<CreationRule>(creationRules.ToList());
            TypeMappingRules = new ReadOnlyCollection<TypeMappingRule>(typeMappingRules.ToList());
        }

        /// <inheritdoc />
        public abstract IBuildLog GetBuildLog();

        /// <inheritdoc />
        public abstract IExecuteStrategy<T> GetExecuteStrategy<T>();

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<CreationRule> CreationRules { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<IPostBuildAction> PostBuildActions { get; }

        /// <inheritdoc />
        public IPropertyResolver PropertyResolver { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<TypeMappingRule> TypeMappingRules { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}