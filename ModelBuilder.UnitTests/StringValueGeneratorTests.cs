﻿using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;

    public class StringValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StringValueGenerator();

            var first = target.Generate(typeof(string), null, executeStrategy);
            var second = target.Generate(typeof(string), null, executeStrategy);

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void GenerateReturnsValidatesTypeSupportTest(Type type, bool supported)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StringValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            if (supported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }
        
        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new StringValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new StringValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}