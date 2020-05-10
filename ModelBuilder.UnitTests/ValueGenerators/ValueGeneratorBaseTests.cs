namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class ValueGeneratorBaseTests
    {
        [Fact]
        public void GenerateForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var value = Guid.NewGuid().ToString();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var value = Guid.NewGuid().ToString();

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(null, typeof(Person));

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(WithConstructorParameters), MemberType = typeof(DataSet))]
        public void GenerateRequestsValueForParameterTest(ParameterInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(true, value);

            sut.Generate(executeStrategy, parameterInfo);

            sut.TypeUsed.Should().Be(parameterInfo.ParameterType);
            sut.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(Person), MemberType = typeof(DataSet))]
        public void GenerateRequestsValueForPropertyInfo(PropertyInfo propertyInfo)
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(true, value);

            sut.Generate(executeStrategy, propertyInfo);

            sut.TypeUsed.Should().Be(propertyInfo.PropertyType);
            sut.ReferenceNameUsed.Should().Be(propertyInfo.Name);
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullParameterInfo()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(executeStrategy, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullPropertyInfo()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(executeStrategy, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.Generate(executeStrategy, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsCachedInstance()
        {
            var sut = new Wrapper(true, null);

            var first = sut.RandomGenerator;
            var second = sut.RandomGenerator;

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(null, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(WithConstructorParameters), MemberType = typeof(DataSet))]
        public void IsMatchRequestsValueForParameterTest(ParameterInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(true, value);

            sut.IsMatch(buildChain, parameterInfo);

            sut.TypeUsed.Should().Be(parameterInfo.ParameterType);
            sut.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(Person), MemberType = typeof(DataSet))]
        public void IsMatchRequestsValueForPropertyTest(PropertyInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(true, value);

            sut.IsMatch(buildChain, parameterInfo);

            sut.TypeUsed.Should().Be(parameterInfo.PropertyType);
            sut.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullParameterInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullPropertyInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(true, value);

            Action action = () => sut.IsMatch(buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsMinimumValue()
        {
            var buildHistory = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildHistory);

            var sut = Substitute.ForPartsOf<ValueGeneratorBase>();

            var actual = sut.Priority;

            actual.Should().Be(int.MinValue);
        }

        private class Wrapper : ValueGeneratorBase
        {
            private readonly bool _isSupported;
            private readonly object _value;

            public Wrapper(bool isSupported, object value)
            {
                _isSupported = isSupported;
                _value = value;
            }

            protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
            {
                TypeUsed = type;
                ReferenceNameUsed = referenceName;

                return _value;
            }

            protected override bool IsMatch(IBuildChain buildChain, Type type, string referenceName)
            {
                TypeUsed = type;
                ReferenceNameUsed = referenceName;

                return _isSupported;
            }

            public IRandomGenerator RandomGenerator => Generator;

            public string ReferenceNameUsed { get; private set; }

            public Type TypeUsed { get; private set; }
        }
    }
}