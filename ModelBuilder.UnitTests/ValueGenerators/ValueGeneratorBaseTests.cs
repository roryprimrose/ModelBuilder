﻿namespace ModelBuilder.UnitTests.ValueGenerators
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
        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(WithConstructorParameters), MemberType = typeof(DataSet))]
        public void GenerateRequestsValueForParameterTest(ParameterInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new GenerateWrapper(true, value);

            target.Generate(parameterInfo, executeStrategy);

            target.TypeUsed.Should().Be(parameterInfo.ParameterType);
            target.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(Person), MemberType = typeof(DataSet))]
        public void GenerateRequestsValueForPropertyInfo(PropertyInfo propertyInfo)
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new GenerateWrapper(true, value);

            target.Generate(propertyInfo, executeStrategy);

            target.TypeUsed.Should().Be(propertyInfo.PropertyType);
            target.ReferenceNameUsed.Should().Be(propertyInfo.Name);
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullParameterInfo()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new GenerateWrapper(true, value);

            Action action = () => target.Generate((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullPropertyInfo()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new GenerateWrapper(true, value);

            Action action = () => target.Generate((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsCachedInstance()
        {
            var target = new GenerateWrapper(true, null);

            var first = target.RandomGenerator;
            var second = target.RandomGenerator;

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void IsSupportedForParameterThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var target = new GenerateWrapper(true, value);

            Action action = () => target.IsSupported(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedForPropertyThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new GenerateWrapper(true, value);

            Action action = () => target.IsSupported(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(WithConstructorParameters), MemberType = typeof(DataSet))]
        public void IsSupportedRequestsValueForParameterTest(ParameterInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenerateWrapper(true, value);

            target.IsSupported(parameterInfo, buildChain);

            target.TypeUsed.Should().Be(parameterInfo.ParameterType);
            target.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(Person), MemberType = typeof(DataSet))]
        public void IsSupportedRequestsValueForPropertyTest(PropertyInfo parameterInfo)
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenerateWrapper(true, value);

            target.IsSupported(parameterInfo, buildChain);

            target.TypeUsed.Should().Be(parameterInfo.PropertyType);
            target.ReferenceNameUsed.Should().Be(parameterInfo.Name);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullParameterInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenerateWrapper(true, value);

            Action action = () => target.IsSupported((ParameterInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullPropertyInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenerateWrapper(true, value);

            Action action = () => target.IsSupported((PropertyInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsMinimumValueTest()
        {
            var buildStrategy = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildStrategy);

            var target = Substitute.ForPartsOf<ValueGeneratorBase>();

            var actual = target.Priority;

            actual.Should().Be(int.MinValue);
        }

        private class GenerateWrapper : ValueGeneratorBase
        {
            private readonly bool _isSupported;
            private readonly object _value;

            public GenerateWrapper(bool isSupported, object value)
            {
                _isSupported = isSupported;
                _value = value;
            }

            protected override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                TypeUsed = type;
                ReferenceNameUsed = referenceName;

                return _value;
            }

            protected override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
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