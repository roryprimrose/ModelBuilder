namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;
    using Xunit.Abstractions;

    public class NumericGenerationTests
    {
        private readonly ITestOutputHelper _output;

        public NumericGenerationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateAllNumberTypes()
        {
            var actual = Model.WriteLog<Numbers>(_output.WriteLine).Create();

            actual.First.Should().NotBe(0);
            actual.Second.Should().NotBe(0);
            actual.Third.Should().NotBe(0);
            actual.Fourth.Should().NotBe(0);
            actual.Fifth.Should().NotBe(0);
            actual.Sixth.Should().NotBe(0);
            actual.Seventh.Should().NotBe(0);
            actual.Eighth.Should().NotBe(0);
            actual.Nineth.Should().NotBe(0);
            actual.Tenth.Should().NotBe(0);
        }

        [Fact]
        public void CanCreateNumericParameters()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>().WriteLog<NumericParameterModel>(_output.WriteLine);

            var actual = config.Create();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateNumericProperties()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<NumericValueGenerator>().WriteLog<NumericPropertyModel>(_output.WriteLine);

            var actual = config.Create();

            actual.Should().NotBeNull();
        }
    }
}