namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class RandomSourceTests
    {
        [Fact]
        public void DifferentSeedsProduceDifferentSequences()
        {
            var first = new RandomSource(1);
            var second = new RandomSource(2);

            first.NextInt64().Should().NotBe(second.NextInt64());
        }

        [Fact]
        public void NextBoolReturnsBothValuesOverManySamples()
        {
            var sut = new RandomSource(12345);

            var values = Enumerable.Range(0, 200).Select(_ => sut.NextBool()).ToList();

            values.Should().Contain(true);
            values.Should().Contain(false);
        }

        [Fact]
        public void NextBytesFillsBuffer()
        {
            var sut = new RandomSource(7);
            var buffer = new byte[32];

            sut.NextBytes(buffer);

            buffer.Should().Contain(b => b != 0);
        }

        [Fact]
        public void NextBytesThrowsWithNullBuffer()
        {
            var sut = new RandomSource(7);

            Action action = () => sut.NextBytes(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextDecimalFullRangeDoesNotThrow()
        {
            var sut = new RandomSource(99);

            Action action = () => sut.NextDecimal();

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(-100, 100)]
        [InlineData(0, 1)]
        [InlineData(-50, -10)]
        public void NextDecimalReturnsValueWithinInclusiveRange(int min, int max)
        {
            var sut = new RandomSource(31);
            decimal minimum = min;
            decimal maximum = max;

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.NextDecimal(minimum, maximum);

                actual.Should().BeGreaterThanOrEqualTo(minimum).And.BeLessThanOrEqualTo(maximum);
            }
        }

        [Fact]
        public void NextDecimalThrowsWhenMinExceedsMax()
        {
            var sut = new RandomSource(1);

            Action action = () => sut.NextDecimal(10m, 5m);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(-1000d, 1000d)]
        [InlineData(0d, 1d)]
        [InlineData(-5.5d, -1.25d)]
        public void NextDoubleReturnsValueWithinInclusiveRange(double min, double max)
        {
            var sut = new RandomSource(17);

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.NextDouble(min, max);

                actual.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
            }
        }

        [Fact]
        public void NextDoubleThrowsWhenMinExceedsMax()
        {
            var sut = new RandomSource(1);

            Action action = () => sut.NextDouble(1d, 0d);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextInt64FullRangeProducesValuesOutsideInt32Range()
        {
            var sut = new RandomSource(4242);

            var values = Enumerable.Range(0, 1000).Select(_ => sut.NextInt64()).ToList();

            values.Should().Contain(value => value > int.MaxValue || value < int.MinValue);
        }

        [Theory]
        [InlineData(long.MinValue, long.MaxValue)]
        [InlineData(-5000000000L, 5000000000L)]
        [InlineData(0L, 1L)]
        public void NextInt64ReturnsValueWithinInclusiveRange(long min, long max)
        {
            var sut = new RandomSource(53);

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.NextInt64(min, max);

                actual.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
            }
        }

        [Fact]
        public void NextInt64ThrowsWhenMinExceedsMax()
        {
            var sut = new RandomSource(1);

            Action action = () => sut.NextInt64(1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(int.MinValue, int.MaxValue)]
        [InlineData(-100, 100)]
        [InlineData(5, 5)]
        public void NextInt32ReturnsValueWithinInclusiveRange(int min, int max)
        {
            var sut = new RandomSource(11);

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.NextInt32(min, max);

                actual.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
            }
        }

        [Fact]
        public void NextInt32ThrowsWhenMinExceedsMax()
        {
            var sut = new RandomSource(1);

            Action action = () => sut.NextInt32(1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(42, 42)]
        public void NextInt32WhenMinEqualsMaxReturnsThatValue(int min, int max)
        {
            var sut = new RandomSource(1);

            var actual = sut.NextInt32(min, max);

            actual.Should().Be(min);
        }

        [Theory]
        [InlineData(ulong.MinValue, ulong.MaxValue)]
        [InlineData(0UL, 10UL)]
        public void NextUInt64ReturnsValueWithinInclusiveRange(ulong min, ulong max)
        {
            var sut = new RandomSource(67);

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.NextUInt64(min, max);

                actual.Should().BeGreaterThanOrEqualTo(min).And.BeLessThanOrEqualTo(max);
            }
        }

        [Fact]
        public void NextUInt64ThrowsWhenMinExceedsMax()
        {
            var sut = new RandomSource(1);

            Action action = () => sut.NextUInt64(10, 5);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void SameSeedProducesIdenticalSequence()
        {
            var first = new RandomSource(123);
            var second = new RandomSource(123);

            var firstValues = ReadSequence(first);
            var secondValues = ReadSequence(second);

            firstValues.Should().Equal(secondValues);
        }

        [Fact]
        public void SeedIsExposedFromDefaultConstructor()
        {
            var sut = new RandomSource();

            var replay = new RandomSource(sut.Seed);

            ReadSequence(replay).Should().Equal(ReadSequence(sut));
        }

        [Fact]
        public void SeedReturnsValueProvidedInConstructor()
        {
            var sut = new RandomSource(8675309);

            sut.Seed.Should().Be(8675309);
        }

        [Fact]
        public void SupportsConcurrentAccessWithoutCorruption()
        {
            var sut = new RandomSource(2024);
            var results = new List<int>[16];

            Parallel.For(
                0,
                results.Length,
                worker =>
                {
                    var local = new List<int>(1000);

                    for (var index = 0; index < 1000; index++)
                    {
                        local.Add(sut.NextInt32(0, 100));
                    }

                    results[worker] = local;
                });

            var all = results.SelectMany(values => values).ToList();

            all.Should().HaveCount(16000);
            all.Should().OnlyContain(value => value >= 0 && value <= 100);
        }

        private static IReadOnlyList<long> ReadSequence(IRandomSource source)
        {
            return Enumerable.Range(0, 50).Select(_ => source.NextInt64()).ToList();
        }
    }
}
