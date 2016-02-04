using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BaseBuildStrategy"/>
    /// class is used to provide the base framework for a build strategy.
    /// </summary>
    public abstract class BaseBuildStrategy : IBuildStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBuildStrategy"/> class.
        /// </summary>
        /// <param name="constructorResolver">The constructor resolver.</param>
        /// <param name="typeCreators">The type creators.</param>
        /// <param name="valueGenerators">The value generators.</param>
        /// <param name="ignoreRules">The ignore rules.</param>
        protected BaseBuildStrategy(IConstructorResolver constructorResolver, IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators, IEnumerable<IgnoreRule> ignoreRules)
        {
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

            ConstructorResolver = constructorResolver;
            TypeCreators = new ReadOnlyCollection<ITypeCreator>(typeCreators.ToList());
            ValueGenerators = new ReadOnlyCollection<IValueGenerator>(valueGenerators.ToList());
            IgnoreRules = new ReadOnlyCollection<IgnoreRule>(ignoreRules.ToList());
        }

        /// <inheritdoc />
        public abstract IExecuteStrategy<T> GetExecuteStrategy<T>();

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}