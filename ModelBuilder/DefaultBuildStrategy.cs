using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DefaultBuildStrategy"/>
    /// class is used to provide the default definition for how to create and update instances of a type.
    /// </summary>
    public class DefaultBuildStrategy : BaseBuildStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBuildStrategy"/> class.
        /// </summary>
        public DefaultBuildStrategy()
            : base(
                DefaultConstructorResolver, DefaultTypeCreators, DefaultValueGenerators,
                GetDefaultIgnoreRules())
        {
        }

        /// <inheritdoc />
        public override IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            var executeStrategy = new DefaultExecuteStrategy<T> {ConstructorResolver = ConstructorResolver};

            foreach (var ignoreRule in IgnoreRules)
            {
                executeStrategy.IgnoreRules.Add(ignoreRule);
            }

            foreach (var typeCreator in TypeCreators)
            {
                executeStrategy.TypeCreators.Add(typeCreator);
            }

            foreach (var valueGenerator in ValueGenerators)
            {
                executeStrategy.ValueGenerators.Add(valueGenerator);
            }

            return executeStrategy;
        }

        /// <summary>
        /// Gets the default ignore rules.
        /// </summary>
        /// <returns>The ignore rules.</returns>
        private static IEnumerable<IgnoreRule> GetDefaultIgnoreRules()
        {
            return new List<IgnoreRule>();
        }

        /// <summary>
        /// Gets the default constructor resolver.
        /// </summary>
        /// <value>The constructor resolver.</value>
        public static IConstructorResolver DefaultConstructorResolver => new DefaultConstructorResolver();

        /// <summary>
        /// Gets the default type creators.
        /// </summary>
        /// <value>The type creators.</value>
        public static IEnumerable<ITypeCreator> DefaultTypeCreators
        {
            get
            {
                yield return new EnumerableTypeCreator();
                yield return new DefaultTypeCreator();
            }
        }

        /// <summary>
        /// Gets the default value generators.
        /// </summary>
        /// <value>The value generators.</value>
        public static IEnumerable<IValueGenerator> DefaultValueGenerators
        {
            get
            {
                yield return new StringValueCreator();
                yield return new BooleanValueGenerator();
                yield return new DateTimeValueGenerator();
                yield return new EnumValueGenerator();
                yield return new NumericValueGenerator();
                yield return new GuidValueGenerator();
            }
        }
    }
}