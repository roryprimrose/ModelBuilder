namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of constructor selection: the greediest public constructor is used for
    ///     <c>Model.Create&lt;T&gt;()</c>, and every public constructor gets a matching typed
    ///     <c>Model.Construct&lt;T&gt;().From(...)</c> overload.
    /// </summary>
    public class ConstructionTests
    {
        [Fact]
        public void CreateUsesParameterlessConstructorWhenAvailable()
        {
            // Model.Create<T>() always prefers the parameterless constructor over any parameterized
            // overload (design.md 5.2 "Compile-time constructor choice"), unlike v8's greedy
            // DefaultConstructorResolver. Use Model.Construct<T>().From(...) to reach the others.
            var actual = Model.Create<MultiConstructorWidget>();

            actual.Name.Should().Be("Default");
            actual.Quantity.Should().Be(0);
            actual.Price.Should().Be(0);
        }

        [Fact]
        public void ConstructFromParameterlessOverloadUsesDefaults()
        {
            var actual = Model.Construct<MultiConstructorWidget>().From();

            actual.Name.Should().Be("Default");
            actual.Quantity.Should().Be(0);
            actual.Price.Should().Be(0);
        }

        [Fact]
        public void ConstructFromSingleArgumentOverloadKeepsSuppliedValue()
        {
            var actual = Model.Construct<MultiConstructorWidget>().From("Explicit");

            actual.Name.Should().Be("Explicit");
            actual.Quantity.Should().Be(0);
        }

        [Fact]
        public void ConstructFromTwoArgumentOverloadKeepsSuppliedValues()
        {
            var actual = Model.Construct<MultiConstructorWidget>().From("Explicit", 5);

            actual.Name.Should().Be("Explicit");
            actual.Quantity.Should().Be(5);
            actual.Price.Should().Be(0);
        }

        [Fact]
        public void ConstructFromThreeArgumentOverloadKeepsAllSuppliedValues()
        {
            var actual = Model.Construct<MultiConstructorWidget>().From("Explicit", 5, 12.5m);

            actual.Name.Should().Be("Explicit");
            actual.Quantity.Should().Be(5);
            actual.Price.Should().Be(12.5m);
        }
    }
}
