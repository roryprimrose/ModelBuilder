namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder.vNext;
    using Xunit;

    public class BuildTargetTests
    {
        [Fact]
        public void ConstructorStoresTypeAndMemberName()
        {
            var sut = new BuildTarget(typeof(string), "FirstName");

            sut.Type.Should().Be(typeof(string));
            sut.MemberName.Should().Be("FirstName");
        }

        [Fact]
        public void ConstructorThrowsWithNullType()
        {
            Action action = () => _ = new BuildTarget(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentMemberName()
        {
            var first = new BuildTarget(typeof(int), "Age");
            var second = new BuildTarget(typeof(int), "Count");

            first.Equals(second).Should().BeFalse();
            (first == second).Should().BeFalse();
            (first != second).Should().BeTrue();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentType()
        {
            var first = new BuildTarget(typeof(int));
            var second = new BuildTarget(typeof(long));

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameTypeAndMemberName()
        {
            var first = new BuildTarget(typeof(int), "Age");
            var second = new BuildTarget(typeof(int), "Age");

            first.Equals(second).Should().BeTrue();
            (first == second).Should().BeTrue();
            (first != second).Should().BeFalse();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void MemberNameIsNullWhenNotProvided()
        {
            var sut = new BuildTarget(typeof(Guid));

            sut.MemberName.Should().BeNull();
        }
    }
}
