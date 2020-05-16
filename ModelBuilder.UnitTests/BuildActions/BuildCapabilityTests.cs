namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class BuildCapabilityTests
    {
        [Fact]
        public void ImplementedByTypeReturnsConstructorParameter()
        {
            var type = typeof(EmailValueGenerator);

            var sut = new BuildCapability(type);

            sut.ImplementedByType.Should().Be(type);
        }

        [Fact]
        public void ThrowsExceptionWithNullImplementedByType()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildCapability(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}