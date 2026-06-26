namespace ModelBuilder.Generator.UnitTests
{
    using System.Collections.Immutable;
    using FluentAssertions;
    using Xunit;

    public class EnumModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new EnumModel("My.Enum", "MyEnum", Members("A"), false);

            sut.Equals("not an enum").Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentFullyQualifiedName()
        {
            var first = new EnumModel("My.First", "MyEnum", Members("A"), false);
            var second = new EnumModel("My.Second", "MyEnum", Members("A"), false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentIsFlags()
        {
            var first = new EnumModel("My.Enum", "MyEnum", Members("A"), false);
            var second = new EnumModel("My.Enum", "MyEnum", Members("A"), true);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentMemberNames()
        {
            var first = new EnumModel("My.Enum", "MyEnum", Members("A"), false);
            var second = new EnumModel("My.Enum", "MyEnum", Members("B"), false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentSourceName()
        {
            var first = new EnumModel("My.Enum", "First", Members("A"), false);
            var second = new EnumModel("My.Enum", "Second", Members("A"), false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new EnumModel("My.Enum", "MyEnum", Members("A", "B"), true);
            var second = new EnumModel("My.Enum", "MyEnum", Members("A", "B"), true);

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new EnumModel("My.Enum", "MyEnum", Members("A", "B"), true);
            var second = new EnumModel("My.Enum", "MyEnum", Members("A", "B"), true);

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var members = Members("A", "B");

            var sut = new EnumModel("My.Enum", "MyEnum", members, true);

            sut.FullyQualifiedName.Should().Be("My.Enum");
            sut.SourceName.Should().Be("MyEnum");
            sut.IsFlags.Should().BeTrue();
            sut.MemberNames.Should().Equal(members);
        }

        private static EquatableArray<string> Members(params string[] names)
        {
            return new EquatableArray<string>(ImmutableArray.Create(names));
        }
    }
}
