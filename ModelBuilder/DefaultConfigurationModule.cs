namespace ModelBuilder
{
    using System;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="DefaultConfigurationModule" />
    ///     class is used to provide the default configuration for building values.
    /// </summary>
    public class DefaultConfigurationModule : IConfigurationModule
    {
        /// <inheritdoc />
        public void Configure(IBuildConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.ConstructorResolver = new DefaultConstructorResolver();
            configuration.PropertyResolver = new DefaultPropertyResolver();

            AddExecuteOrderRules(configuration);
            AddTypeCreators(configuration);
            AddValueGenerators(configuration);
        }

        private static void AddExecuteOrderRules(IBuildConfiguration configuration)
        {
            // Populate personal properties in a specific order for scenarios where a value generator may use the values in order to set other values
            configuration.AddExecuteOrderRule(PropertyExpression.Gender, 9600);
            configuration.AddExecuteOrderRule(PropertyExpression.FirstName, 9580);
            configuration.AddExecuteOrderRule(PropertyExpression.LastName, 9560);
            configuration.AddExecuteOrderRule(PropertyExpression.Domain, 9550);
            configuration.AddExecuteOrderRule(PropertyExpression.Email, 9540);
            configuration.AddExecuteOrderRule(PropertyExpression.Country, 9400);
            configuration.AddExecuteOrderRule(PropertyExpression.State, 9390);
            configuration.AddExecuteOrderRule(PropertyExpression.City, 9380);
            configuration.AddExecuteOrderRule(PropertyExpression.PostCode, 9370);
            configuration.AddExecuteOrderRule(PropertyExpression.TimeZone, 9360);
            configuration.AddExecuteOrderRule(PropertyExpression.DateOfBirth, 9340);
            configuration.AddExecuteOrderRule(PropertyExpression.Age, 9320);

            configuration.AddExecuteOrderRule(x => x.PropertyType.IsEnum, 4000);
            configuration.AddExecuteOrderRule(x => x.PropertyType.IsValueType, 3000);

            // Populate strings before other reference types
            configuration.AddExecuteOrderRule(x => x.PropertyType == typeof(string), 2000);
            configuration.AddExecuteOrderRule(x => x.PropertyType.IsClass, 1000);
        }

        private static void AddTypeCreators(IBuildConfiguration configuration)
        {
            configuration.AddTypeCreator<ArrayTypeCreator>();
            configuration.AddTypeCreator<EnumerableTypeCreator>();
            configuration.AddTypeCreator<DefaultTypeCreator>();
        }

        private static void AddValueGenerators(IBuildConfiguration configuration)
        {
            configuration.AddValueGenerator<AddressValueGenerator>();
            configuration.AddValueGenerator<AgeValueGenerator>();
            configuration.AddValueGenerator<BooleanValueGenerator>();
            configuration.AddValueGenerator<CityValueGenerator>();
            configuration.AddValueGenerator<CompanyValueGenerator>();
            configuration.AddValueGenerator<CountryValueGenerator>();
            configuration.AddValueGenerator<CountValueGenerator>();
            configuration.AddValueGenerator<CultureValueGenerator>();
            configuration.AddValueGenerator<DateOfBirthValueGenerator>();
            configuration.AddValueGenerator<DateTimeValueGenerator>();
            configuration.AddValueGenerator<DomainNameValueGenerator>();
            configuration.AddValueGenerator<EmailValueGenerator>();
            configuration.AddValueGenerator<EnumValueGenerator>();
            configuration.AddValueGenerator<FirstNameValueGenerator>();
            configuration.AddValueGenerator<GenderValueGenerator>();
            configuration.AddValueGenerator<GuidValueGenerator>();
            configuration.AddValueGenerator<IPAddressValueGenerator>();
            configuration.AddValueGenerator<LastNameValueGenerator>();
            configuration.AddValueGenerator<NumericValueGenerator>();
            configuration.AddValueGenerator<PhoneValueGenerator>();
            configuration.AddValueGenerator<PostCodeValueGenerator>();
            configuration.AddValueGenerator<StateValueGenerator>();
            configuration.AddValueGenerator<StringValueGenerator>();
            configuration.AddValueGenerator<SuburbValueGenerator>();
            configuration.AddValueGenerator<TimeZoneValueGenerator>();
            configuration.AddValueGenerator<TimeZoneInfoValueGenerator>();
            configuration.AddValueGenerator<UriValueGenerator>();
        }
    }
}