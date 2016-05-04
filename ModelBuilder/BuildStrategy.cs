using System;
using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BuildStrategy"/>
    /// class is used to provide a basic build strategy.
    /// </summary>
    public class BuildStrategy : BuildStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStrategyBase"/> class.
        /// </summary>
        /// <param name="constructorResolver">The constructor resolver.</param>
        /// <param name="typeCreators">The type creators.</param>
        /// <param name="valueGenerators">The value generators.</param>
        /// <param name="ignoreRules">The ignore rules.</param>
        /// <param name="executeOrderRules">The execute order rules.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorResolver"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="ignoreRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeOrderRules"/> parameter is null.</exception>
        public BuildStrategy(IConstructorResolver constructorResolver, IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators, IEnumerable<IgnoreRule> ignoreRules,
            IEnumerable<ExecuteOrderRule> executeOrderRules)
            : base(constructorResolver, typeCreators, valueGenerators, ignoreRules, executeOrderRules)
        {
        }

        /// <inheritdoc />
        public override IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            return this.With<DefaultExecuteStrategy<T>>();
        }
    }
}