using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DefaultBuildStrategy"/>
    /// class is used to provide the default definition for how to create and update instances of a type.
    /// </summary>
    public class DefaultBuildStrategy : BuildStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBuildStrategy"/> class.
        /// </summary>
        public DefaultBuildStrategy()
            : base(
                DefaultConstructorResolver, DefaultTypeCreators, DefaultValueGenerators,
                DefaultIgnoreRules)
        {
        }

        private DefaultBuildStrategy(IEnumerable<ITypeCreator> typeCreators,
            IEnumerable<IValueGenerator> valueGenerators, IEnumerable<IgnoreRule> ignoreRules)
            : base(
                DefaultConstructorResolver, typeCreators, valueGenerators,
                ignoreRules)
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
        /// Gets the default constructor resolver.
        /// </summary>
        /// <value>The constructor resolver.</value>
        public static IConstructorResolver DefaultConstructorResolver => new DefaultConstructorResolver();

        /// <summary>
        /// Gets the default ignore rules.
        /// </summary>
        /// <value>The ignore rules.</value>
        public static IEnumerable<IgnoreRule> DefaultIgnoreRules { get; } = new List<IgnoreRule>();

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
                yield return new AgeValueGenerator();
                yield return new BooleanValueGenerator();
                yield return new DateOfBirthValueGenerator();
                yield return new DateTimeValueGenerator();
                yield return new EmailValueGenerator();
                yield return new EnumValueGenerator();
                yield return new GuidValueGenerator();
                yield return new IPAddressValueGenerator();
                yield return new NumericValueGenerator();
                yield return new StringValueGenerator();
            }
        }
    }
}