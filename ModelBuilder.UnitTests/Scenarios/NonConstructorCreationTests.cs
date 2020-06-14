namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class NonConstructorCreationTests
    {
        [Fact]
        public void CanCreateViaSingletonProperty()
        {
            var actual = Model.Create<Singleton>();

            actual.Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanCreateViaStaticFactoryMethodWithCreatedParameters()
        {
            var actual = Model.Create<FactoryWithValue>();

            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateViaStaticFactoryMethodWithoutParameters()
        {
            var actual = Model.Create<FactoryItem>();

            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateViaStaticFactoryMethodWithProvidedParameters()
        {
            var value = Guid.NewGuid();

            var actual = Model.Create<FactoryWithValue>(value);

            actual.Value.Should().Be(value);
        }
    }
}