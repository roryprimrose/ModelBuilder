namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class StructTypeCreatorTests
    {
        [Fact]
        public void CanCreateReturnsFalseForEnum()
        {
            var type = typeof(Gender);
            var referenceName = Guid.NewGuid().ToString();

            var config = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunCanCreate(config, buildChain, type, referenceName);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsFalseForReferenceType()
        {
            var type = typeof(Person);
            var referenceName = Guid.NewGuid().ToString();

            var config = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunCanCreate(config, buildChain, type, referenceName);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanCreateReturnsTrueForStruct()
        {
            var type = typeof(StructModel);
            var referenceName = Guid.NewGuid().ToString();

            var config = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunCanCreate(config, buildChain, type, referenceName);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullType()
        {
            var referenceName = Guid.NewGuid().ToString();

            var config = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            Action action = () => sut.RunCanCreate(config, buildChain, null!, referenceName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateInstanceReturnsNewValue()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var referenceName = Guid.NewGuid().ToString();
            var type = typeof(StructModel);

            var sut = new Wrapper();

            var actual = sut.RunCreateInstance(executeStrategy, type, referenceName);

            actual.Should().BeOfType<StructModel>();
        }

        [Fact]
        public void CreateInstanceReturnsNewValueWithArguments()
        {
            var key = Guid.NewGuid();
            var value = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var referenceName = Guid.NewGuid().ToString();
            var type = typeof(KeyValuePair<Guid, Person>);

            var sut = new Wrapper();

            var actual = sut.RunCreateInstance(executeStrategy, type, referenceName, key, value);

            actual.Should().BeOfType<KeyValuePair<Guid, Person>>();

            var pair = actual.As<KeyValuePair<Guid, Person>>();

            pair.Key.Should().Be(key);
            pair.Value.Should().Be(value);
        }

        [Fact]
        public void CreateInstanceThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var referenceName = Guid.NewGuid().ToString();

            var sut = new Wrapper();

            Action action = () => sut.RunCreateInstance(executeStrategy, null!, referenceName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateReturnsValueWithoutModification()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var expected = new StructModel
            {
                Email = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid(),
                LastName = Guid.NewGuid().ToString()
            };

            var sut = new StructTypeCreator();

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PriorityReturnsHigherThanDefaultTypeCreator()
        {
            var other = new DefaultTypeCreator();

            var sut = new StructTypeCreator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : StructTypeCreator
        {
            public bool RunCanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
                string referenceName)
            {
                return base.CanCreate(configuration, buildChain, type, referenceName);
            }

            public object RunCreateInstance(IExecuteStrategy executeStrategy, Type type, string referenceName,
                params object?[]? args)
            {
                return base.CreateInstance(executeStrategy, type, referenceName, args)!;
            }
        }
    }
}