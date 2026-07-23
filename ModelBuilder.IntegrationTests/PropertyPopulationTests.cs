namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of settable-property population, including members inherited from a base
    ///     class.
    /// </summary>
    public class PropertyPopulationTests
    {
        [Fact]
        public void CreatePopulatesOwnAndInheritedSettableProperties()
        {
            var actual = Model.Create<Invoice>();

            actual.Id.Should().NotBeEmpty();
            actual.CreatedOn.Should().NotBe(default(System.DateTime));
            actual.Reference.Should().NotBeNullOrWhiteSpace();
            actual.Total.Should().NotBe(0);
        }
    }
}
