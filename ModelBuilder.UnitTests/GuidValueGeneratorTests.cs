namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class GuidValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var nullFound = false;
            var valueFound = false;

            var target = new GuidValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (Guid?)target.Generate(typeof(Guid?), null, null);

                if (value == null)
                {
                    nullFound = true;
                }
                else if (value.Value != Guid.Empty)
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(Guid), true)]
        [InlineData(typeof(Guid?), true)]
        [InlineData(typeof(string), false)]
        public void GenerateEvaluatesWhetherTypeIsSupportedTest(Type type, bool supportedType)
        {
            var target = new GuidValueGenerator();

            Action action = () => target.Generate(type, null, null);

            if (supportedType)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Fact]
        public void GenerateReturnsRandomGuidValueTest()
        {
            var target = new GuidValueGenerator();

            var first = (Guid)target.Generate(typeof(Guid), null, null);
            var second = (Guid)target.Generate(typeof(Guid), null, null);

            first.Should().NotBeEmpty();
            second.Should().NotBeEmpty();
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(Guid), true)]
        [InlineData(typeof(Guid?), true)]
        [InlineData(typeof(string), false)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supportedType)
        {
            var target = new GuidValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supportedType);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(Guid.NewGuid().ToString());

            var target = new GuidValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}