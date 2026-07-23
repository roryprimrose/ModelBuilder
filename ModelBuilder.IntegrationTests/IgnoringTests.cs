namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of member exclusion via typed <c>Ignoring</c>, type+name <c>Ignoring</c>, and
    ///     predicate-based <c>IgnoringAny</c>, confirming ignored members are left at their default while
    ///     other members still populate normally.
    /// </summary>
    public class IgnoringTests
    {
        [Fact]
        public void CreateLeavesTypedIgnoredMemberAtDefault()
        {
            var actual = Model.Ignoring<Credentials>(x => x.Password).Create<Credentials>();

            // An ignored member is never assigned, so it retains its field-initializer default
            // (string.Empty) rather than receiving a built value.
            actual.Password.Should().BeEmpty();
            actual.Username.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateLeavesTypeAndNameIgnoredMemberAtDefault()
        {
            var actual = Model.Ignoring(typeof(Credentials), nameof(Credentials.Password)).Create<Credentials>();

            actual.Password.Should().BeEmpty();
            actual.Username.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateLeavesPredicateMatchedMemberAtDefault()
        {
            var actual = Model.IgnoringAny(member => member.Name.EndsWith("Secret", System.StringComparison.Ordinal))
                .Create<Credentials>();

            actual.ApiKeySecret.Should().BeEmpty();
            actual.Username.Should().NotBeNullOrWhiteSpace();
            actual.Password.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateAppliesAllIgnoreRulesFromConfigurationModule()
        {
            var actual = Model.UsingModule<IntegrationTestModule>().Create<Credentials>();

            actual.Password.Should().BeEmpty();
            actual.ApiKeySecret.Should().BeEmpty();
            actual.Username.Should().NotBeNullOrWhiteSpace();
        }
    }
}
