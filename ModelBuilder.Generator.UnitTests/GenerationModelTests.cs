namespace ModelBuilder.Generator.UnitTests
{
    using System.Collections.Immutable;
    using FluentAssertions;
    using Xunit;

    public class GenerationModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());

            sut.Equals("not a generation model").Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentBuilders()
        {
            var first = new GenerationModel(BuildersFor("My.First"), Enums(), Strings("System.Int32"), Collections());
            var second = new GenerationModel(BuildersFor("My.Second"), Enums(), Strings("System.Int32"), Collections());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentNullableUnderlyingTypes()
        {
            var first = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());
            var second = new GenerationModel(Builders(), Enums(), Strings("System.Int64"), Collections());

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());
            var second = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());
            var second = new GenerationModel(Builders(), Enums(), Strings("System.Int32"), Collections());

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void IsEmptyReturnsFalseWhenAnyContentPresent()
        {
            var sut = new GenerationModel(
                Builders(),
                Empty<EnumModel>(),
                Empty<string>(),
                Empty<CollectionModel>());

            sut.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void IsEmptyReturnsTrueWhenAllEmpty()
        {
            var sut = new GenerationModel(
                Empty<BuildableModel>(),
                Empty<EnumModel>(),
                Empty<string>(),
                Empty<CollectionModel>());

            sut.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var builders = Builders();
            var enums = Enums();
            var nullableUnderlyingTypes = Strings("System.Int32");
            var collections = Collections();

            var sut = new GenerationModel(builders, enums, nullableUnderlyingTypes, collections);

            sut.Builders.Should().Equal(builders);
            sut.Enums.Should().Equal(enums);
            sut.NullableUnderlyingTypes.Should().Equal(nullableUnderlyingTypes);
            sut.Collections.Should().Equal(collections);
        }

        private static EquatableArray<BuildableModel> Builders()
        {
            return BuildersFor("My.Type");
        }

        private static EquatableArray<BuildableModel> BuildersFor(string fullyQualifiedName)
        {
            var members = new EquatableArray<MemberModel>(
                ImmutableArray.Create(new MemberModel("Value", "System.String")));
            var constructors = new EquatableArray<ConstructorModel>(ImmutableArray<ConstructorModel>.Empty);

            return new EquatableArray<BuildableModel>(
                ImmutableArray.Create(
                    new BuildableModel(fullyQualifiedName, "MyTypeBuilder", members, members, constructors)));
        }

        private static EquatableArray<CollectionModel> Collections()
        {
            return new EquatableArray<CollectionModel>(
                ImmutableArray.Create(
                    new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", false)));
        }

        private static EquatableArray<T> Empty<T>()
            where T : System.IEquatable<T>
        {
            return new EquatableArray<T>(ImmutableArray<T>.Empty);
        }

        private static EquatableArray<EnumModel> Enums()
        {
            var members = new EquatableArray<string>(ImmutableArray.Create("A"));

            return new EquatableArray<EnumModel>(
                ImmutableArray.Create(new EnumModel("My.Enum", "MyEnum", members, false)));
        }

        private static EquatableArray<string> Strings(params string[] values)
        {
            return new EquatableArray<string>(ImmutableArray.Create(values));
        }
    }
}
