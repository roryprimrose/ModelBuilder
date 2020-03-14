namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class NumericGenerationTests
    {
        [Fact]
        public void CanCreateNumericParameters()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>();

            var actual = config.Create<NumericParameterTest>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateNumericProperties()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>();

            var actual = config.Create<NumericPropertyTest>();

            actual.Should().NotBeNull();
        }
    }
}