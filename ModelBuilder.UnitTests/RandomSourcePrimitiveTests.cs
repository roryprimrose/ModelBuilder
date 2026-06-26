namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class RandomSourcePrimitiveTests
    {
        [Theory]
        [InlineData((byte)0, (byte)255)]
        [InlineData((byte)10, (byte)20)]
        [InlineData((byte)7, (byte)7)]
        public void NextByteReturnsValueWithinInclusiveRange(byte min, byte max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextByte(min, max);

            actual.Should().BeInRange(min, max);
        }

        [Fact]
        public void NextDecimalWhenMinEqualsMaxReturnsThatValue()
        {
            var sut = new RandomSource(5);

            var actual = sut.NextDecimal(7.5m, 7.5m);

            actual.Should().Be(7.5m);
        }

        [Fact]
        public void NextDoubleFullRangeReturnsFiniteValueWithinRange()
        {
            var sut = new RandomSource(5);

            var actual = sut.NextDouble(double.MinValue, double.MaxValue);

            double.IsInfinity(actual).Should().BeFalse();
            actual.Should().BeInRange(double.MinValue, double.MaxValue);
        }

        [Fact]
        public void NextDoubleWhenMinEqualsMaxReturnsThatValue()
        {
            var sut = new RandomSource(5);

            var actual = sut.NextDouble(3.5, 3.5);

            actual.Should().Be(3.5);
        }

        [Theory]
        [InlineData(short.MinValue, short.MaxValue)]
        [InlineData((short)-10, (short)10)]
        [InlineData((short)4, (short)4)]
        public void NextInt16ReturnsValueWithinInclusiveRange(short min, short max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextInt16(min, max);

            actual.Should().BeInRange(min, max);
        }

        [Theory]
        [InlineData(sbyte.MinValue, sbyte.MaxValue)]
        [InlineData((sbyte)-5, (sbyte)5)]
        [InlineData((sbyte)2, (sbyte)2)]
        public void NextSByteReturnsValueWithinInclusiveRange(sbyte min, sbyte max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextSByte(min, max);

            actual.Should().BeInRange(min, max);
        }

        [Theory]
        [InlineData(0f, 10f)]
        [InlineData(-100f, 100f)]
        public void NextSingleReturnsValueWithinInclusiveRange(float min, float max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextSingle(min, max);

            actual.Should().BeInRange(min, max);
        }

        [Fact]
        public void NextSingleWhenMinEqualsMaxReturnsThatValue()
        {
            var sut = new RandomSource(5);

            var actual = sut.NextSingle(2.5f, 2.5f);

            actual.Should().Be(2.5f);
        }

        [Theory]
        [InlineData(ushort.MinValue, ushort.MaxValue)]
        [InlineData((ushort)10, (ushort)20)]
        [InlineData((ushort)6, (ushort)6)]
        public void NextUInt16ReturnsValueWithinInclusiveRange(ushort min, ushort max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextUInt16(min, max);

            actual.Should().BeInRange(min, max);
        }

        [Theory]
        [InlineData(uint.MinValue, uint.MaxValue)]
        [InlineData(10u, 20u)]
        [InlineData(8u, 8u)]
        public void NextUInt32ReturnsValueWithinInclusiveRange(uint min, uint max)
        {
            var sut = new RandomSource(5);

            var actual = sut.NextUInt32(min, max);

            actual.Should().BeInRange(min, max);
        }
    }
}
