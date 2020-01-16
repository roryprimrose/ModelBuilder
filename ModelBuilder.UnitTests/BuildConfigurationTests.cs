namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class BuildConfigurationTests
    {
        [Fact]
        public void CreatedWithDefaultValues()
        {
            var sut = new BuildConfiguration();

            sut.TypeMappingRules.Should().BeEmpty();
            sut.IgnoreRules.Should().BeEmpty();
            sut.TypeCreators.Should().BeEmpty();
            sut.ValueGenerators.Should().BeEmpty();
            sut.ExecuteOrderRules.Should().BeEmpty();
            sut.CreationRules.Should().BeEmpty();
            sut.PostBuildActions.Should().BeEmpty();
            sut.ConstructorResolver.Should().NotBeNull();
            sut.PropertyResolver.Should().NotBeNull();
        }
    }
}