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
                DefaultIgnoreRules, DefaultExecuteOrderRules)
        {
        }
        
        /// <inheritdoc />
        public override IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            return this.With<DefaultExecuteStrategy<T>>();
        }

        /// <summary>
        /// Gets the default constructor resolver.
        /// </summary>
        /// <value>The constructor resolver.</value>
        public static IConstructorResolver DefaultConstructorResolver => new DefaultConstructorResolver();

        /// <summary>
        /// Gets the default execute order rules.
        /// </summary>
        /// <value>The execute order rules.</value>
        public static IEnumerable<ExecuteOrderRule> DefaultExecuteOrderRules
        {
            get
            {
                yield return new ExecuteOrderRule((type, name) => type.IsClass, 100);
                // Populate strings before other reference types
                yield return new ExecuteOrderRule(typeof(string), null, 200);
                yield return new ExecuteOrderRule((type, name) => type.IsValueType, 300);
                yield return new ExecuteOrderRule((type, name) => type.IsEnum, 400);

            }
        }

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