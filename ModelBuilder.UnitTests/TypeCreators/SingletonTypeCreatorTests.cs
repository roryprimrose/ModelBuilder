namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class SingletonTypeCreatorTests
    {
        [Theory]
        [InlineData(CacheLevel.Global)]
        [InlineData(CacheLevel.PerInstance)]
        [InlineData(CacheLevel.None)]
        public void CacheLevelReturnsConstructorValue(CacheLevel cacheLevel)
        {
            var sut = new SingletonTypeCreator(cacheLevel);

            sut.CacheLevel.Should().Be(cacheLevel);
        }

        [Fact]
        public void CanCreateReturnsFalseForInterface()
        {
            var type = typeof(ITestItem);

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsFalseWhenConstructorWithArgumentsFound()
        {
            var type = typeof(SimpleConstructor);
            var constructorInfo = type.GetConstructors().First();

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsFalseWhenDefaultConstructorFound()
        {
            var type = typeof(TestItem);
            var constructorInfo = type.GetConstructors().First();

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsTrueWhenNoPublicConstructorAndStaticSingletonPropertyExists()
        {
            var type = typeof(Singleton);
            ConstructorInfo? constructorInfo = null;

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CreateReturnsValueFromSingleton()
        {
            var type = typeof(Singleton);
            ConstructorInfo? constructorInfo = null;

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);
            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.Create(executeStrategy, type);

            actual.Should().BeOfType<Singleton>();
            actual.As<Singleton>().Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateReturnsValueWithoutCache()
        {
            var type = typeof(Singleton);
            ConstructorInfo? constructorInfo = null;

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);
            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new SingletonTypeCreator(CacheLevel.None);

            var actual = sut.Create(executeStrategy, type);

            actual.Should().BeOfType<Singleton>();
            actual.As<Singleton>().Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulateReturnsProvidedInstance()
        {
            var expected = Model.Create<Simple>()!;
            var buildChain = new BuildHistory();
            var constructorResolver = new DefaultConstructorResolver(CacheLevel.PerInstance);

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);
            configuration.ConstructorResolver.Returns(constructorResolver);

            var sut = new SingletonTypeCreator(CacheLevel.PerInstance);

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }
    }
}