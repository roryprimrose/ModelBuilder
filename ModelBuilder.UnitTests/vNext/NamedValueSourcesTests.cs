namespace ModelBuilder.UnitTests.vNext
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using ModelBuilder.Data;
    using Xunit;

    public class NamedValueSourcesTests
    {
        [Fact]
        public void BuildResolvesFirstNameFromEntityData()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "FirstName");

            var known = TestData.FemaleNames.Concat(TestData.MaleNames);
            known.Should().Contain(actual);
        }

        [Fact]
        public void BuildResolvesEmailContainingAtSymbol()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "Email");

            actual.Should().Contain("@");
        }

        [Fact]
        public void BuildResolvesCompanyFromEntityData()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "Company");

            TestData.Companies.Should().Contain(actual);
        }

        [Fact]
        public void BuildResolvesCountryFromEntityData()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "Country");

            TestData.Locations.Select(location => location.Country).Should().Contain(actual);
        }

        [Fact]
        public void BuildMatchesNamesCaseInsensitivelyAndBySubstring()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "ContactEmailAddress");

            actual.Should().Contain("@");
        }

        [Fact]
        public void BuildFallsBackToRandomStringForUnmatchedMember()
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), "SomethingArbitrary");

            actual.Should().NotBeNullOrEmpty();
        }

        private sealed class Sample
        {
        }
    }
}
