namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The <see cref="BuildStrategyBase"/>
    /// class is used to provide the base framework for a build strategy.
    /// </summary>
    public abstract class BuildStrategyBase : IBuildStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStrategyBase"/> class.
        /// </summary>
        /// <param name="constructorResolver">The constructor resolver.</param>
        /// <param name="creationRules">The creation rules.</param>
        /// <param name="typeCreators">The type creators.</param>
        /// <param name="valueGenerators">The value generators.</param>
        /// <param name="ignoreRules">The ignore rules.</param>
        /// <param name="executeOrderRules">The execute order rules.</param>
        /// <param name="buildLog">The build log.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorResolver"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creationRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="ignoreRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeOrderRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildLog"/> parameter is null.</exception>
        protected BuildStrategyBase(
            IConstructorResolver constructorResolver,
            IEnumerable<CreationRule> creationRules,
            IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators,
            IEnumerable<IgnoreRule> ignoreRules,
            IEnumerable<ExecuteOrderRule> executeOrderRules,
            IBuildLog buildLog)
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

            if (valueGenerators == null)
            {
                throw new ArgumentNullException(nameof(valueGenerators));
            }

            if (ignoreRules == null)
            {
                throw new ArgumentNullException(nameof(ignoreRules));
            }

            if (executeOrderRules == null)
            {
                throw new ArgumentNullException(nameof(executeOrderRules));
            }

            if (buildLog == null)
            {
                throw new ArgumentNullException(nameof(buildLog));
            }

            ConstructorResolver = constructorResolver;
            TypeCreators = new ReadOnlyCollection<ITypeCreator>(typeCreators.ToList());
            ValueGenerators = new ReadOnlyCollection<IValueGenerator>(valueGenerators.ToList());
            IgnoreRules = new ReadOnlyCollection<IgnoreRule>(ignoreRules.ToList());
            ExecuteOrderRules = new ReadOnlyCollection<ExecuteOrderRule>(executeOrderRules.ToList());
            CreationRules = new ReadOnlyCollection<CreationRule>(creationRules.ToList());
            BuildLog = buildLog;
        }

        /// <inheritdoc />
        public abstract IExecuteStrategy<T> GetExecuteStrategy<T>();

        /// <inheritdoc />
        public IBuildLog BuildLog
        {
            get;
        }

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver
        {
            get;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<CreationRule> CreationRules
        {
            get;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules
        {
            get;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<IgnoreRule> IgnoreRules
        {
            get;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<ITypeCreator> TypeCreators
        {
            get;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<IValueGenerator> ValueGenerators
        {
            get;
        }
    }
}