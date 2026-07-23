namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of interface-to-concrete-type mapping via <c>Model.Mapping&lt;TSource,TTarget&gt;()</c>
    ///     and via a configuration module registering the same mapping.
    /// </summary>
    public class MappingTests
    {
        [Fact]
        public void CreatePopulatesInterfaceMemberUsingFluentMapping()
        {
            var actual = Model.Mapping<IEngine, DieselEngine>().Create<Vehicle>();

            actual.Model.Should().NotBeNullOrWhiteSpace();
            actual.Engine.Should().NotBeNull();
            actual.Engine.Should().BeOfType<DieselEngine>();
            actual.Engine!.Name.Should().NotBeNullOrWhiteSpace();
            actual.Engine.Horsepower.Should().NotBe(0);
        }

        [Fact]
        public void CreatePopulatesInterfaceMemberUsingConfigurationModuleMapping()
        {
            var actual = Model.UsingModule<IntegrationTestModule>().Create<Vehicle>();

            actual.Engine.Should().NotBeNull();
            actual.Engine.Should().BeOfType<DieselEngine>();
        }

        [Fact]
        public void CreatePopulatesInterfaceMemberUsingNonGenericMapping()
        {
            var actual = Model.Mapping(typeof(IEngine), typeof(DieselEngine)).Create<Vehicle>();

            actual.Engine.Should().NotBeNull();
            actual.Engine.Should().BeOfType<DieselEngine>();
        }
    }
}
