namespace ModelBuilder.UnitTests.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildSteps;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class CircularReferenceBuildStepTests
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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(type, executeStrategy);

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

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(string);

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((ParameterInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((PropertyInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(string);

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((Type) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsMaximumValue()
        {
            var sut = new CircularReferenceBuildStep();

            sut.Priority.Should().Be(int.MaxValue);
        }
    }
}