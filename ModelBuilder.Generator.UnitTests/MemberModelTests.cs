namespace ModelBuilder.Generator.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class MemberModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new MemberModel("Name", "System.String");

            sut.Equals("not a member").Should().BeFalse();
        }

        [Fact]
        public void EqualsObjectReturnsTrueForEqualValue()
        {
            var first = new MemberModel("Name", "System.String", "default");
            object second = new MemberModel("Name", "System.String", "default");

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentDefaultLiteral()
        {
            var first = new MemberModel("Name", "System.String", "a");
            var second = new MemberModel("Name", "System.String", "b");

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentName()
        {
            var first = new MemberModel("First", "System.String");
            var second = new MemberModel("Second", "System.String");

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentTypeName()
        {
            var first = new MemberModel("Name", "System.String");
            var second = new MemberModel("Name", "System.Int32");

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new MemberModel("Name", "System.String", "default");
            var second = new MemberModel("Name", "System.String", "default");

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new MemberModel("Name", "System.String", "default");
            var second = new MemberModel("Name", "System.String", "default");

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var sut = new MemberModel("Name", "System.String", "default");

            sut.Name.Should().Be("Name");
            sut.TypeName.Should().Be("System.String");
            sut.DefaultLiteral.Should().Be("default");
        }
    }
}
