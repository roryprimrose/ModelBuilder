namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using FluentAssertions.Execution;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class NullableValueTests
    {
        [Fact]
        public void CanConfigureNullValueGeneration()
        {
            var config = Model.UsingDefaultConfiguration()
                .UpdateValueGenerator<NumericValueGenerator>(x => x.AllowNull = true);

            for (var index = 0; index < 10000; index++)
            {
                var actual = config.Create<NumericPropertyModel>()!;

                if (actual.PropNullableInt == null)
                {
                    // We are happy with this
                    return;
                }
            }

            throw new AssertionFailedException("Null value was expected to be generated but none were");
        }

        [Fact]
        public void DoesNotGenerateNullNumericValuesByDefault()
        {
            var config = Model.UsingDefaultConfiguration()
                .UpdateValueGenerator<NumericValueGenerator>(x => x.AllowNull = false);

            for (var index = 0; index < 100; index++)
            {
                var actual = config.Create<NumericPropertyModel>()!;

                actual.PropNullableByte.Should().NotBeNull();
                actual.PropNullableDecimal.Should().NotBeNull();
                actual.PropNullableDouble.Should().NotBeNull();
                actual.PropNullableFloat.Should().NotBeNull();
                actual.PropNullableInt.Should().NotBeNull();
                actual.PropNullableLong.Should().NotBeNull();
                actual.PropNullableSbyte.Should().NotBeNull();
                actual.PropNullableShort.Should().NotBeNull();
                actual.PropNullableUint.Should().NotBeNull();
                actual.PropNullableUlong.Should().NotBeNull();
                actual.PropNullableUshort.Should().NotBeNull();
            }
        }
    }
}