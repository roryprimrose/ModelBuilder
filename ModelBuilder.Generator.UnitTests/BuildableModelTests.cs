namespace ModelBuilder.Generator.UnitTests
{
    using System.Collections.Immutable;
    using FluentAssertions;
    using Xunit;

    public class BuildableModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());

            sut.Equals("not a buildable").Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentBuilderName()
        {
            var first = new BuildableModel("My.Type", "FirstBuilder", Members("Value"), Members("Value"), Constructors());
            var second = new BuildableModel("My.Type", "SecondBuilder", Members("Value"), Members("Value"), Constructors());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentConstructorParameters()
        {
            var first = new BuildableModel("My.Type", "MyTypeBuilder", Members("First"), Members("Value"), Constructors());
            var second = new BuildableModel("My.Type", "MyTypeBuilder", Members("Second"), Members("Value"), Constructors());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentFullyQualifiedName()
        {
            var first = new BuildableModel("My.First", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());
            var second = new BuildableModel("My.Second", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentMembers()
        {
            var first = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("First"), Constructors());
            var second = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Second"), Constructors());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());
            var second = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());
            var second = new BuildableModel("My.Type", "MyTypeBuilder", Members("Value"), Members("Value"), Constructors());

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var ctorParameters = Members("Ctor");
            var members = Members("Member");
            var constructors = Constructors();

            var sut = new BuildableModel("My.Type", "MyTypeBuilder", ctorParameters, members, constructors);

            sut.FullyQualifiedName.Should().Be("My.Type");
            sut.BuilderName.Should().Be("MyTypeBuilder");
            sut.ConstructorParameters.Should().Equal(ctorParameters);
            sut.Members.Should().Equal(members);
            sut.AllConstructors.Should().Equal(constructors);
        }

        private static EquatableArray<ConstructorModel> Constructors()
        {
            var parameters = new EquatableArray<MemberModel>(
                ImmutableArray.Create(new MemberModel("value", "System.String")));
            var docLines = new EquatableArray<string>(ImmutableArray.Create("doc"));

            return new EquatableArray<ConstructorModel>(ImmutableArray.Create(new ConstructorModel(parameters, docLines)));
        }

        private static EquatableArray<MemberModel> Members(string name)
        {
            return new EquatableArray<MemberModel>(ImmutableArray.Create(new MemberModel(name, "System.String")));
        }
    }
}
