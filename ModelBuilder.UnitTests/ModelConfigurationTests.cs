namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelConfigurationTests
    {
        [Fact]
        public void IgnoringExtractsMemberNameFromExpression()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Ignoring<Sample>(x => x.Name);

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void IgnoringThrowsWithNullExpression()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring<Sample>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsWhenExpressionIsNotMemberAccess()
        {
            var sut = new ModelConfiguration();

            Action action = () => sut.Ignoring<Sample>(x => x.ToString());

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MappingReturnsSameConfigurationForChaining()
        {
            var sut = new ModelConfiguration();

            var actual = sut.Mapping<IThing, Thing>();

            actual.Should().BeSameAs(sut);
        }

        private interface IThing
        {
        }

        private sealed class Sample
        {
            public string? Name { get; set; }
        }

        private sealed class Thing : IThing
        {
        }
    }
}
