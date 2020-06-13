namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultTypeCreatorTests
    {
        [Fact]
        public void CanCreateReturnsFalseWhenNoPublicConstructorFound()
        {
            var type = typeof(FactoryItem);
            ConstructorInfo? constructorInfo = null;

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new DefaultTypeCreator();

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsTrueWhenTypeHasPublicConstructor()
        {
            var type = typeof(Person);
            var constructorInfo = typeof(Person).GetConstructors().First();

            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var buildChain = Substitute.For<IBuildChain>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, type).Returns(type);
            constructorResolver.Resolve(type, Arg.Any<object?[]?>()).Returns(constructorInfo);

            var sut = new DefaultTypeCreator();

            var actual = sut.CanCreate(configuration, buildChain, type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithCreatedParameters()
        {
            var constructorResolver = new DefaultConstructorResolver(CacheLevel.PerInstance);
            var constructorInfo = constructorResolver.Resolve(typeof(SimpleConstructor), null)!;
            var args = new object[]
            {
                new SlimModel()
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.CreateParameters(constructorInfo).Returns(args);
            configuration.ConstructorResolver.Returns(constructorResolver);
            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new DefaultTypeCreator();

            var actual = sut.Create(executeStrategy, typeof(SimpleConstructor), null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithProvidedArguments()
        {
            var constructorInfo = typeof(Person).GetConstructors().First();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            configuration.ConstructorResolver.Returns(constructorResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);
            constructorResolver.Resolve(typeof(Person), Arg.Any<object?[]?>()).Returns(constructorInfo);

            var args = new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount
            };

            var sut = new DefaultTypeCreator();

            var actual = sut.Create(executeStrategy, typeof(Person), args);

            actual.Should().BeOfType<Person>();

            var person = (Person) actual!;

            person.FirstName.Should().Be((string) args[0]);
            person.LastName.Should().Be((string) args[1]);
            person.DOB.Should().Be((DateTime) args[2]);
            person.IsActive.Should().Be((bool) args[3]);
            person.Id.Should().Be((Guid) args[4]);
            person.Priority.Should().Be((int) args[5]);
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithResolvedDefaultConstructorWhenArgumentsAreEmpty()
        {
            var constructorResolver = new DefaultConstructorResolver(CacheLevel.PerInstance);
            var args = Array.Empty<object>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.ConstructorResolver.Returns(constructorResolver);
            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new DefaultTypeCreator();

            var actual = sut.Create(executeStrategy, typeof(Person), args);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithResolvedDefaultConstructorWhenArgumentsIsNull()
        {
            var constructorResolver = new DefaultConstructorResolver(CacheLevel.PerInstance);

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.ConstructorResolver.Returns(constructorResolver);
            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new DefaultTypeCreator();

            var actual = sut.Create(executeStrategy, typeof(Person), null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateThrowsExceptionWhenConstructorNotFound()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            constructorResolver.Resolve(Arg.Any<Type>()).Returns((ConstructorInfo?) null);
            configuration.ConstructorResolver.Returns(constructorResolver);
            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new DefaultTypeCreator();

            Action action = () => sut.Create(executeStrategy, typeof(Person), null);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoTypeNotSupported()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new DefaultTypeCreator();

            Action action = () => sut.Create(executeStrategy, typeof(Stream));

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenTypeConstructorsDoNotSupportProvidedArguments()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var args = new object[]
            {
                Guid.NewGuid()
            };

            var sut = new DefaultTypeCreator();

            Action action = () => sut.Create(executeStrategy, typeof(Person), args);

            action.Should().Throw<NotSupportedException>();
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

            var sut = new DefaultTypeCreator();

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }
    }
}