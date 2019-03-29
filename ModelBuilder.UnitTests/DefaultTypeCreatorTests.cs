namespace ModelBuilder.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultTypeCreatorTests
    {
        [Fact]
        [SuppressMessage("Microsoft.Design",
            "CA1825",
            Justification = "The Array.Empty<T> is not available on net452.")]
        public void CreateReturnsInstanceCreatedWithDefaultConstructorWhenArgumentsAreEmptyTest()
        {
            var buildChain = new BuildHistory();
            var args = new object[] { };

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
    }
}