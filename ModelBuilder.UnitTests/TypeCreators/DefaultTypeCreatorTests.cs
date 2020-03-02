namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultTypeCreatorTests
    {
        [Fact]
        public void CreateReturnsInstanceCreatedWithDefaultConstructorWhenArgumentsAreEmptyTest()
        {
            var buildChain = new BuildHistory();
            var args = Array.Empty<object>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), executeStrategy, args);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithDefaultConstructorWhenArgumentsAreNullTest()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), executeStrategy, null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithMatchingParameterConstructorTest()
        {
            var buildChain = new BuildHistory();
            var constructorResolver = new DefaultConstructorResolver();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);
            configuration.ConstructorResolver.Returns(constructorResolver);

            var args = new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount
            };

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), executeStrategy, args);

            actual.Should().BeOfType<Person>();

            var person = (Person) actual;

            person.FirstName.Should().Be((string) args[0]);
            person.LastName.Should().Be((string) args[1]);
            person.DOB.Should().Be((DateTime) args[2]);
            person.IsActive.Should().Be((bool) args[3]);
            person.Id.Should().Be((Guid) args[4]);
            person.Priority.Should().Be((int) args[5]);
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoAppropriateConstructorFoundTest()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var args = new object[]
            {
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid(), Environment.TickCount
            };

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Person), executeStrategy, args);

            action.Should().Throw<MissingMemberException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoTypeNotSupportedTest()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Stream), executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateReturnsProvidedInstance()
        {
            var expected = Model.Create<Simple>();
            var buildChain = new BuildHistory();
            var constructorResolver = new DefaultConstructorResolver();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);
            configuration.ConstructorResolver.Returns(constructorResolver);

            var target = new DefaultTypeCreator();

            var actual = target.Populate(expected, executeStrategy);

            actual.Should().Be(expected);
        }
    }
}