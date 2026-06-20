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
    }
}
