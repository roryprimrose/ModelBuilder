namespace ModelBuilder.Generator.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class CollectionModelTests
    {
        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", false);

            sut.Equals("not a collection").Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentElementType()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "First", "Value", false);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Source", "Second", "Value", false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentKeyCanBeNull()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", false);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", true);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentKind()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", false);
            var second = new CollectionModel(CollectionKind.Set, "Slot", "Source", "Element", "Value", false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentSlotType()
        {
            var first = new CollectionModel(CollectionKind.List, "First", "Source", "Element", "Value", false);
            var second = new CollectionModel(CollectionKind.List, "Second", "Source", "Element", "Value", false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentSourceName()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "First", "Element", "Value", false);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Second", "Element", "Value", false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentValueType()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "First", false);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Second", false);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForSameValues()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", true);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", true);

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualValues()
        {
            var first = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", true);
            var second = new CollectionModel(CollectionKind.List, "Slot", "Source", "Element", "Value", true);

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void PropertiesReturnConstructorArguments()
        {
            var sut = new CollectionModel(CollectionKind.Set, "Slot", "Source", "Element", "Value", true);

            sut.Kind.Should().Be(CollectionKind.Set);
            sut.SlotType.Should().Be("Slot");
            sut.SourceName.Should().Be("Source");
            sut.ElementType.Should().Be("Element");
            sut.ValueType.Should().Be("Value");
            sut.KeyCanBeNull.Should().BeTrue();
        }
    }
}
