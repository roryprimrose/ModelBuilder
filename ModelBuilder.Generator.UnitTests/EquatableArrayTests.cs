namespace ModelBuilder.Generator.UnitTests
{
    using System.Collections;
    using System.Collections.Immutable;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class EquatableArrayTests
    {
        [Fact]
        public void CountReturnsLength()
        {
            var sut = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));

            sut.Count.Should().Be(3);
        }

        [Fact]
        public void CountReturnsZeroWhenDefault()
        {
            var sut = default(EquatableArray<int>);

            sut.Count.Should().Be(0);
        }

        [Fact]
        public void EnumeratesElements()
        {
            var sut = new EquatableArray<int>(ImmutableArray.Create(4, 5, 6));

            sut.ToList().Should().ContainInOrder(4, 5, 6);
        }

        [Fact]
        public void EnumeratesEmptyWhenDefault()
        {
            var sut = default(EquatableArray<int>);

            sut.ToList().Should().BeEmpty();
        }

        [Fact]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var sut = new EquatableArray<int>(ImmutableArray.Create(1));

            sut.Equals("not an array").Should().BeFalse();
        }

        [Fact]
        public void EqualsObjectReturnsTrueForEqualBoxedValue()
        {
            var first = new EquatableArray<int>(ImmutableArray.Create(1, 2));
            object second = new EquatableArray<int>(ImmutableArray.Create(1, 2));

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentSequences()
        {
            var first = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));
            var second = new EquatableArray<int>(ImmutableArray.Create(1, 2, 4));

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseWhenOnlyOneIsDefault()
        {
            var populated = new EquatableArray<int>(ImmutableArray.Create(1));
            var defaultArray = default(EquatableArray<int>);

            populated.Equals(defaultArray).Should().BeFalse();
            defaultArray.Equals(populated).Should().BeFalse();
        }

        [Fact]
        public void EqualsReturnsTrueForEqualSequences()
        {
            var first = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));
            var second = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void EqualsReturnsTrueWhenBothDefault()
        {
            var first = default(EquatableArray<int>);
            var second = default(EquatableArray<int>);

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void GetHashCodeMatchesForEqualSequences()
        {
            var first = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));
            var second = new EquatableArray<int>(ImmutableArray.Create(1, 2, 3));

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void GetHashCodeReturnsZeroWhenDefault()
        {
            var sut = default(EquatableArray<int>);

            sut.GetHashCode().Should().Be(0);
        }

        [Fact]
        public void IndexerReturnsElementAtPosition()
        {
            var sut = new EquatableArray<int>(ImmutableArray.Create(7, 8, 9));

            sut[1].Should().Be(8);
        }

        [Fact]
        public void NonGenericEnumeratorIteratesElements()
        {
            var sut = new EquatableArray<int>(ImmutableArray.Create(1, 2));

            var enumerator = ((IEnumerable)sut).GetEnumerator();
            var items = new System.Collections.Generic.List<object?>();

            while (enumerator.MoveNext())
            {
                items.Add(enumerator.Current);
            }

            items.Should().ContainInOrder(1, 2);
        }
    }
}
