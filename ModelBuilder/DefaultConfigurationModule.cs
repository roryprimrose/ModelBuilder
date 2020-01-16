namespace ModelBuilder
{
    using System;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

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

        private void AddExecuteOrderRules(IBuildConfiguration configuration)
        {
            configuration.ExecuteOrderRules.Add(
                new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsEnum, 4000));
            configuration.ExecuteOrderRules.Add(
                new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsValueType, 3000));

            // Populate personal properties in a specific order for scenarios where a value generator may use the values in order to set other values
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.Gender, 2600));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.FirstName, 2580));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.LastName, 2560));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.Domain, 2550));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.Email, 2540));

            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.Country, 2400));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.State, 2390));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.City, 2380));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.PostCode, 2370));
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, null, PropertyExpression.TimeZone, 2360));

            // Populate strings before other reference types
            configuration.ExecuteOrderRules.Add(new ExecuteOrderRule(null, typeof(string), (string) null, 2000));
            configuration.ExecuteOrderRules.Add(
                new ExecuteOrderRule((declaringType, propertyType, name) => propertyType.IsClass, 1000));
        }

        private void AddTypeCreators(IBuildConfiguration configuration)
        {
            configuration.AddTypeCreator<ArrayTypeCreator>();
            configuration.AddTypeCreator<EnumerableTypeCreator>();
            configuration.AddTypeCreator<DefaultTypeCreator>();
        }

        private void AddValueGenerators(IBuildConfiguration configuration)
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