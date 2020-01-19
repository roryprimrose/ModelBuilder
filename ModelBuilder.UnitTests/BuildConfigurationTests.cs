﻿namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class BuildConfigurationTests
    {
        [Fact]
        public void CanSetConstructorResolver()
        {
            var expected = Substitute.For<IConstructorResolver>();

            var sut = new BuildConfiguration {ConstructorResolver = expected};

            var actual = sut.ConstructorResolver;

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanSetPropertyResolver()
        {
            var expected = Substitute.For<IPropertyResolver>();

            var sut = new BuildConfiguration {PropertyResolver = expected};

            var actual = sut.PropertyResolver;

            actual.Should().Be(expected);
        }

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