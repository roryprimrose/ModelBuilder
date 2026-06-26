namespace ModelBuilder.Generator.UnitTests
{
    using System.Collections.Immutable;
    using FluentAssertions;
    using Xunit;

    public class ConstructorModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new ConstructorModel(Parameters("value"), DocLines("doc"));

            sut.Equals("not a constructor").Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentDocLines()
        {
            var first = new ConstructorModel(Parameters("value"), DocLines("first"));
            var second = new ConstructorModel(Parameters("value"), DocLines("second"));

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentParameters()
        {
            var first = new ConstructorModel(Parameters("first"), DocLines("doc"));
            var second = new ConstructorModel(Parameters("second"), DocLines("doc"));

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new ConstructorModel(Parameters("value"), DocLines("doc"));
            var second = new ConstructorModel(Parameters("value"), DocLines("doc"));

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new ConstructorModel(Parameters("value"), DocLines("doc"));
            var second = new ConstructorModel(Parameters("value"), DocLines("doc"));

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var parameters = Parameters("value");
            var docLines = DocLines("doc");

            var sut = new ConstructorModel(parameters, docLines);

            sut.Parameters.Should().Equal(parameters);
            sut.DocLines.Should().Equal(docLines);
        }

        private static EquatableArray<string> DocLines(string line)
        {
            return new EquatableArray<string>(ImmutableArray.Create(line));
        }

        private static EquatableArray<MemberModel> Parameters(string name)
        {
            return new EquatableArray<MemberModel>(ImmutableArray.Create(new MemberModel(name, "System.String")));
        }
    }
}
