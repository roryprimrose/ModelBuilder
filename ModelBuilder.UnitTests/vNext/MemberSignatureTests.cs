namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class MemberSignatureTests
    {
        [Fact]
        public void ConstructorStoresValues()
        {
            var sut = new MemberSignature(typeof(Uri), "Host", typeof(string));

            sut.DeclaringType.Should().Be(typeof(Uri));
            sut.Name.Should().Be("Host");
            sut.MemberType.Should().Be(typeof(string));
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public void ConstructorThrowsWhenAnyArgumentIsNull(bool nullDeclaringType, bool nullName, bool nullMemberType)
        {
            var declaringType = nullDeclaringType ? null : typeof(Uri);
            var name = nullName ? null : "Host";
            var memberType = nullMemberType ? null : typeof(string);

            Action action = () => _ = new MemberSignature(declaringType!, name!, memberType!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EqualsReturnsTrueForEquivalentSignatures()
        {
            var first = new MemberSignature(typeof(Uri), "Host", typeof(string));
            var second = new MemberSignature(typeof(Uri), "Host", typeof(string));

            first.Equals(second).Should().BeTrue();
            (first == second).Should().BeTrue();
            (first != second).Should().BeFalse();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
