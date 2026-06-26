namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder;
    using ModelBuilder.Data;
    using Xunit;

    public class BuiltInValueSourcesTests
    {
        public static IEnumerable<object[]> PrimitiveTypes()
        {
            yield return new object[] { typeof(bool) };
            yield return new object[] { typeof(byte) };
            yield return new object[] { typeof(sbyte) };
            yield return new object[] { typeof(short) };
            yield return new object[] { typeof(ushort) };
            yield return new object[] { typeof(int) };
            yield return new object[] { typeof(uint) };
            yield return new object[] { typeof(long) };
            yield return new object[] { typeof(ulong) };
            yield return new object[] { typeof(float) };
            yield return new object[] { typeof(double) };
            yield return new object[] { typeof(decimal) };
            yield return new object[] { typeof(char) };
            yield return new object[] { typeof(string) };
            yield return new object[] { typeof(Guid) };
            yield return new object[] { typeof(DateTime) };
            yield return new object[] { typeof(DateTimeOffset) };
            yield return new object[] { typeof(TimeSpan) };
            yield return new object[] { typeof(Uri) };
            yield return new object[] { typeof(Version) };
            yield return new object[] { typeof(byte[]) };
        }

        [Theory]
        [MemberData(nameof(PrimitiveTypes))]
        public void DefaultRegistryRegistersInBoxType(Type type)
        {
            BuiltInValueSources.Default.RegisteredTypes.Should().Contain(type);
        }

        [Fact]
        public void DefaultReturnsSharedRegistry()
        {
            BuiltInValueSources.Default.Should().BeSameAs(BuiltInValueSources.Default);
        }

        [Fact]
        public void Int32SourceProducesValueResolvableFromContext()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<int>(out var source).Should().BeTrue();

            var first = source!.Create(context, new BuildTarget(typeof(int)));
            var second = source.Create(context, new BuildTarget(typeof(int)));

            // Two draws from a seeded source should differ, proving it is actually generating values.
            (first == second).Should().BeFalse();
        }

        [Fact]
        public void StringSourceProducesNonEmptyValue()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<string>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(string)));

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GuidSourceProducesNonEmptyValue()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<Guid>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(Guid)));

            actual.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void UriSourceProducesHostFromDomainData()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<Uri>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(Uri)));

            // The URI host is drawn from the curated domain data set rather than a synthetic placeholder.
            actual.Scheme.Should().Be("https");
            TestData.Domains.Should().Contain(actual.Host);
        }

        [Fact]
        public void ByteArraySourceProducesNonEmptyBuffer()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<byte[]>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(byte[])));

            actual.Should().NotBeNull();
            actual!.Length.Should().BeInRange(1, 16);
        }

        [Fact]
        public void CharSourceProducesAlphanumericValue()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<char>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(char)));

            char.IsLetterOrDigit(actual).Should().BeTrue();
        }

        [Fact]
        public void MiddleNameSourceProducesName()
        {
            var registry = BuiltInValueSources.CreateNamedRegistry();
            registry.TryGet<string>("MiddleName", out var source).Should().BeTrue();

            var actual = source!.Create(
                new BuildContext(new RandomSource(42)),
                new BuildTarget(typeof(string), "MiddleName"));

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TimeSpanSourceProducesValueWithinADay()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<TimeSpan>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(TimeSpan)));

            actual.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
            actual.Should().BeLessThanOrEqualTo(TimeSpan.FromMinutes(1440));
        }

        [Fact]
        public void VersionSourceProducesVersion()
        {
            var context = new BuildContext(new RandomSource(42));

            context.TryResolveValueSource<Version>(out var source);

            var actual = source!.Create(context, new BuildTarget(typeof(Version)));

            actual.Should().NotBeNull();
            actual!.Major.Should().BeInRange(0, 20);
        }
    }
}
