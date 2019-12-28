namespace ModelBuilder
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="DefaultBuildStrategyCompiler" />
    ///     class provides a <see cref="IBuildStrategyCompiler" /> with the default configuration for creating an
    ///     <see cref="IBuildStrategy" />.
    /// </summary>
    public class DefaultBuildStrategyCompiler : BuildStrategyCompiler
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultBuildStrategyCompiler" /> class.
        /// </summary>
        public DefaultBuildStrategyCompiler()
        {
            ConstructorResolver = new DefaultConstructorResolver();
            PropertyResolver = new DefaultPropertyResolver();

            foreach (var creator in DefaultTypeCreators)
            {
                TypeCreators.Add(creator);
            }

            foreach (var generator in DefaultValueGenerators)
            {
                ValueGenerators.Add(generator);
            }

            foreach (var rule in DefaultExecuteOrderRules)
            {
                ExecuteOrderRules.Add(rule);
            }
        }

        private static IEnumerable<ExecuteOrderRule> DefaultExecuteOrderRules
        {
            get
            {
                yield return new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsEnum,
                    4000);
                yield return new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsValueType,
                    3000);

                // Populate personal properties in a specific order for scenarios where a value generator may use the values in order to set other values
                yield return new ExecuteOrderRule(null, null, PropertyExpression.Gender, 2600);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.FirstName, 2580);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.LastName, 2560);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.Domain, 2550);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.Email, 2540);

                yield return new ExecuteOrderRule(null, null, PropertyExpression.Country, 2400);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.State, 2390);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.City, 2380);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.PostCode, 2370);
                yield return new ExecuteOrderRule(null, null, PropertyExpression.TimeZone, 2360);

                // Populate strings before other reference types
                yield return new ExecuteOrderRule(null, typeof(string), (string) null, 2000);
                yield return new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsClass,
                    1000);
            }
        }

        private static IEnumerable<ITypeCreator> DefaultTypeCreators
        {
            get
            {
                yield return new ArrayTypeCreator();
                yield return new EnumerableTypeCreator();
                yield return new DefaultTypeCreator();
            }
        }

        private static IEnumerable<IValueGenerator> DefaultValueGenerators
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
                yield return new CultureValueGenerator();
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
                yield return new TimeZoneInfoValueGenerator();
                yield return new UriValueGenerator();
            }
        }
    }
}