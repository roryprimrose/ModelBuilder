namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of <c>Model.Populate</c> filling in an existing instance's settable members,
    ///     combined with an ignore rule to confirm the excluded member is left untouched.
    /// </summary>
    public class PopulateTests
    {
        [Fact]
        public void PopulateFillsSettableMembersOfExistingInstance()
        {
            var instance = new Widget();

            var actual = Model.Populate(instance);

            actual.Should().BeSameAs(instance);
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Price.Should().NotBe(0);
        }

        [Fact]
        public void PopulateAppliesIgnoreRuleFromConfiguration()
        {
            var instance = new Credentials();

            var actual = Model.Ignoring<Credentials>(x => x.Password).Populate(instance);

            actual.Password.Should().BeEmpty();
            actual.Username.Should().NotBeNullOrWhiteSpace();
        }
    }
}
