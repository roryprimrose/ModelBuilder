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
        /// <param name="buildLog">The build log.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorResolver"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerators"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="ignoreRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeOrderRules"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildLog"/> parameter is null.</exception>
        public BuildStrategy(IConstructorResolver constructorResolver, IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators, IEnumerable<IgnoreRule> ignoreRules,
            IEnumerable<ExecuteOrderRule> executeOrderRules, IBuildLog buildLog)
            : base(constructorResolver, typeCreators, valueGenerators, ignoreRules, executeOrderRules, buildLog)
        {
        }

        /// <inheritdoc />
        public override IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            return this.With<DefaultExecuteStrategy<T>>();
        }
    }
}