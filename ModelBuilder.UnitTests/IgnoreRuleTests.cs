using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class IgnoreRuleTests
    {
        [Fact]
        public void ReturnsConstructorValuesTest()
        {
            var type = typeof (string);
            var name = Guid.NewGuid().ToString();

            var target = new IgnoreRule(type, name);

            target.TargetType.Should().Be(type);
            target.PropertyName.Should().Be(name);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithEmptyNameTest()
        {
            Action action = () => new IgnoreRule(typeof (string), string.Empty);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNameTest()
        {
            Action action = () => new IgnoreRule(typeof (string), null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeTest()
        {
            var name = Guid.NewGuid().ToString();

            Action action = () => { new IgnoreRule(null, name); };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithWhiteSpaceNameTest()
        {
            Action action = () => new IgnoreRule(typeof (string), "  ");

            action.Should().Throw<ArgumentException>();
        }
    }
}