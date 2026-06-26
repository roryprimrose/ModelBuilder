namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelIgnoringTests
    {
        [Fact]
        public void IgnoringByTypeAndNameReturnsConfiguration()
        {
            var actual = Model.Ignoring(typeof(Sample), nameof(Sample.Name));

            actual.Should().NotBeNull();
        }

        [Fact]
        public void IgnoringByTypeAndNameThrowsWithNullDeclaringType()
        {
            Action action = () => Model.Ignoring(null!, nameof(Sample.Name));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringByTypeAndNameThrowsWithNullMemberName()
        {
            Action action = () => Model.Ignoring(typeof(Sample), null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringAnyReturnsConfiguration()
        {
            var actual = Model.IgnoringAny(member => member.Name == nameof(Sample.Name));

            actual.Should().NotBeNull();
        }

        [Fact]
        public void IgnoringAnyThrowsWithNullPredicate()
        {
            Action action = () => Model.IgnoringAny(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private sealed class Sample
        {
            public string? Name { get; set; }
        }
    }
}
