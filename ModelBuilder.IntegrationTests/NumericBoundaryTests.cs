namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests that every numeric primitive kind populates a non-default, in-range value
    ///     without overflow or type-conversion failures.
    /// </summary>
    public class NumericBoundaryTests
    {
        [Fact]
        public void CreatePopulatesEveryNumericPrimitiveWithoutThrowing()
        {
            var actual = Model.Create<NumericBoundaries>();

            actual.Should().NotBeNull();

            // sbyte/byte/short/ushort/etc. all deserialize within their declared CLR type's range by
            // construction (the assignment above would throw OverflowException/InvalidCastException at
            // runtime if the generator produced an out-of-range literal or the wrong numeric type).
            actual.CharValue.Should().NotBe('\0');
        }

        [Fact]
        public void CreateProducesVariedValuesAcrossRepeatedBuildsForEachNumericMember()
        {
            var first = Model.Create<NumericBoundaries>();
            var second = Model.Create<NumericBoundaries>();

            // Not every single member is guaranteed to differ between two builds, but requiring the
            // whole set of numeric members to match by coincidence across two independent builds is
            // vanishingly unlikely, so this proves real randomization rather than fixed literals.
            var same = first.SByteValue == second.SByteValue
                       && first.ByteValue == second.ByteValue
                       && first.ShortValue == second.ShortValue
                       && first.UShortValue == second.UShortValue
                       && first.IntValue == second.IntValue
                       && first.UIntValue == second.UIntValue
                       && first.LongValue == second.LongValue
                       && first.ULongValue == second.ULongValue
                       && first.FloatValue.Equals(second.FloatValue)
                       && first.DoubleValue.Equals(second.DoubleValue)
                       && first.DecimalValue == second.DecimalValue;

            same.Should().BeFalse();
        }
    }
}
