namespace ModelBuilder.UnitTests
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
        public void CanSetParameterResolver()
        {
            var expected = Substitute.For<IParameterResolver>();

            var sut = new BuildConfiguration {ParameterResolver = expected};

            var actual = sut.ParameterResolver;

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
        public void CanSetTypeResolver()
        {
            var expected = Substitute.For<ITypeResolver>();

            var sut = new BuildConfiguration {TypeResolver = expected};

            var actual = sut.TypeResolver;

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
            sut.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            sut.ConstructorResolver.As<DefaultConstructorResolver>().CacheLevel.Should().Be(CacheLevel.PerInstance);
            sut.ParameterResolver.Should().BeOfType<DefaultParameterResolver>();
            sut.ParameterResolver.As<DefaultParameterResolver>().CacheLevel.Should().Be(CacheLevel.PerInstance);
            sut.PropertyResolver.Should().BeOfType<DefaultPropertyResolver>();
            sut.PropertyResolver.As<DefaultPropertyResolver>().CacheLevel.Should().Be(CacheLevel.PerInstance);
            sut.TypeResolver.Should().BeOfType<DefaultTypeResolver>();
        }
    }
}