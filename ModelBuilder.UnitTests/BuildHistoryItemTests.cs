namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class BuildHistoryItemTests
    {
        [Fact]
        public void CapabilitiesReturnsEmptyByDefault()
        {
            var value = Guid.NewGuid().ToString();

            var actual = new BuildHistoryItem(value);

            actual.Capabilities.Should().BeEmpty();
        }

        [Fact]
        public void ThrowsExceptionWithNullInstance()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildHistoryItem(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValueReturnsConstructorValue()
        {
            var value = Guid.NewGuid().ToString();

            var actual = new BuildHistoryItem(value);

            actual.Value.Should().Be(value);
        }
    }
}