﻿namespace ModelBuilder.UnitTests.TypeCreators
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

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), null, strategy, args);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithDefaultConstructorWhenArgumentsAreNullTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), null, executeStrategy, null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithMatchingParameterConstructorTest()
        {
            var buildChain = new BuildHistory();
            var resolver = new DefaultConstructorResolver();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var config = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(config);
            config.ConstructorResolver.Returns(resolver);

            executeStrategy.BuildChain.Returns(buildChain);

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

            var actual = target.Create(typeof(Person), null, executeStrategy, args);

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

            executeStrategy.BuildChain.Returns(buildChain);

            var args = new object[]
            {
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid(), Environment.TickCount
            };

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Person), null, executeStrategy, args);

            action.Should().Throw<MissingMemberException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoTypeNotSupportedTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Stream), null, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateReturnsProvidedInstance()
        {
            var expected = Model.Create<Simple>();
            var buildChain = new BuildHistory();

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            var actual = target.Populate(expected, strategy);

            actual.Should().Be(expected);
        }
    }
}