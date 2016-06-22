namespace ModelBuilder
{
    using System.Collections.Generic;

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
                DefaultConstructorResolver,
                DefaultCreationRules,
                DefaultTypeCreators,
                DefaultValueGenerators,
                DefaultIgnoreRules,
                DefaultExecuteOrderRules,
                DefaultPostBuildActions,
                DefaultBuildLog)
        {
        }

        /// <inheritdoc />
        public override IExecuteStrategy<T> GetExecuteStrategy<T>()
        {
            return this.With<DefaultExecuteStrategy<T>>();
        }

        /// <summary>
        /// Gets the default build log.
        /// </summary>
        /// <value>The build log.</value>
        public static IBuildLog DefaultBuildLog => new DefaultBuildLog();

        /// <summary>
        /// Gets the default constructor resolver.
        /// </summary>
        /// <value>The constructor resolver.</value>
        public static IConstructorResolver DefaultConstructorResolver => new DefaultConstructorResolver();

        /// <summary>
        /// Gets the default creation rules.
        /// </summary>
        /// <value>The creation rules.</value>
        public static IEnumerable<CreationRule> DefaultCreationRules
        {
            get;
        } = new List<CreationRule>();

        /// <summary>
        /// Gets the default execute order rules.
        /// </summary>
        /// <value>The execute order rules.</value>
        public static IEnumerable<ExecuteOrderRule> DefaultExecuteOrderRules
        {
            get
            {
                yield return new ExecuteOrderRule((type, name) => type.IsEnum, 4000);
                yield return new ExecuteOrderRule((type, name) => type.IsValueType, 3000);

                // Populate personal properties in a specific order for scenarios where a value generator may use the values in order to set other values
                yield return new ExecuteOrderRule(null, PropertyExpression.Gender, 2600);
                yield return new ExecuteOrderRule(null, PropertyExpression.FirstName, 2580);
                yield return new ExecuteOrderRule(null, PropertyExpression.LastName, 2560);
                yield return new ExecuteOrderRule(null, PropertyExpression.Email, 2540);

                // Populate strings before other reference types
                yield return new ExecuteOrderRule(typeof(string), (string)null, 2000);
                yield return new ExecuteOrderRule((type, name) => type.IsClass, 1000);
            }
        }

        /// <summary>
        /// Gets the default post-build actions.
        /// </summary>
        /// <value>The post-build actions.</value>
        public static IEnumerable<IPostBuildAction> DefaultPostBuildActions
        {
            get;
        } = new List<IPostBuildAction>();

        /// <summary>
        /// Gets the default ignore rules.
        /// </summary>
        /// <value>The ignore rules.</value>
        public static IEnumerable<IgnoreRule> DefaultIgnoreRules
        {
            get;
        } = new List<IgnoreRule>();

        /// <summary>
        /// Gets the default type creators.
        /// </summary>
        /// <value>The type creators.</value>
        public static IEnumerable<ITypeCreator> DefaultTypeCreators
        {
            get
            {
                yield return new ArrayTypeCreator();
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
                yield return new AddressValueGenerator();
                yield return new AgeValueGenerator();
                yield return new BooleanValueGenerator();
                yield return new CityValueGenerator();
                yield return new CompanyValueGenerator();
                yield return new CountryValueGenerator();
                yield return new CountValueGenerator();
                yield return new DateOfBirthValueGenerator();
                yield return new DateTimeValueGenerator();
                yield return new DomainNameValueGenerator();
                yield return new EmailValueGenerator();
                yield return new EnumValueGenerator();
                yield return new FirstNameValueGenerator();
                yield return new GenderValueGenerator();
                yield return new GuidValueGenerator();
                yield return new IPAddressValueGenerator();
                yield return new LastNameValueGenerator();
                yield return new NumericValueGenerator();
                yield return new PhoneValueGenerator();
                yield return new PostCodeValueGenerator();
                yield return new StateValueGenerator();
                yield return new StringValueGenerator();
                yield return new SuburbValueGenerator();
                yield return new TimeZoneValueGenerator();
                yield return new UriValueGenerator();
            }
        }
    }
}