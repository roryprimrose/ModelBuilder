namespace ModelBuilder.UnitTests
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NullableValueSourceTests
    {
        [Fact]
        public void CreateNeverReturnsNullWhenNullPercentageIsZero()
        {
            ValueSource<int>.Instance = new ConstantInt32Source(5);

            try
            {
                var options = new BuildContextOptions
                {
                    NullPercentage = 0
                };
                var context = new BuildContext(new RandomSource(1), null, options);
                var sut = new NullableValueSource<int>();
                var target = new BuildTarget(typeof(int?));

                var values = Enumerable.Range(0, 100)
                    .Select(_ => sut.Create(context, target))
                    .ToList();

                values.Should().OnlyContain(value => value == 5);
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
        }

        [Fact]
        public void CreateReturnsNullWhenNoInnerSourceRegistered()
        {
            ValueSource<int>.Instance = null;

            var options = new BuildContextOptions
            {
                NullPercentage = 0
            };
            var emptySources = new ValueSourceRegistry();
            var context = new BuildContext(new RandomSource(1), null, options, null, emptySources);
            var sut = new NullableValueSource<int>();

            var actual = sut.Create(context, new BuildTarget(typeof(int?)));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsNullWhenNullPercentageIsHundred()
        {
            ValueSource<int>.Instance = new ConstantInt32Source(5);

            try
            {
                var options = new BuildContextOptions
                {
                    NullPercentage = 100
                };
                var context = new BuildContext(new RandomSource(1), null, options);
                var sut = new NullableValueSource<int>();

                var actual = sut.Create(context, new BuildTarget(typeof(int?)));

                actual.Should().BeNull();
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
        }

        private sealed class ConstantInt32Source : IValueSource<int>
        {
            private readonly int _value;

            public ConstantInt32Source(int value)
            {
                _value = value;
            }

            public int Create(IBuildContext context, in BuildTarget target)
            {
                return _value;
            }
        }
    }
}
