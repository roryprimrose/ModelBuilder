namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class CircularReferenceBuildActionTests
    {
        [Fact]
        public void BuildForParameterReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(string);

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(string);
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsMaximumValue()
        {
            var sut = new CircularReferenceBuildAction();

            sut.Priority.Should().Be(int.MaxValue);
        }
    }
}