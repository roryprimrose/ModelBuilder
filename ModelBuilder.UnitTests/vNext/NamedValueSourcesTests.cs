namespace ModelBuilder.UnitTests.vNext
{
    using System;
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

        [Theory]
        [InlineData("FullName")]
        [InlineData("Name")]
        [InlineData("DisplayName")]
        public void BuildResolvesFullNameAsTwoParts(string memberName)
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), memberName);

            // A full name composes a first and last name separated by a space.
            actual.Should().Contain(" ");
            actual.Trim().Should().Be(actual);
        }

        [Theory]
        [InlineData("UserName")]
        [InlineData("Username")]
        [InlineData("Login")]
        public void BuildResolvesUserNameAsLowercaseDottedValue(string memberName)
        {
            var context = new BuildContext(new RandomSource(7));

            var actual = context.Build<string>(typeof(Sample), memberName);

            actual.Should().NotBeNullOrEmpty();
            actual.Should().Contain(".");
            actual.Should().Be(actual.ToLowerInvariant());
        }

        [Fact]
        public void BuildResolvesAgeWithinHumanPlausibleRange()
        {
            var context = new BuildContext(new RandomSource(7));

            for (var index = 0; index < 100; index++)
            {
                var actual = context.Build<int>(typeof(Sample), "Age");

                actual.Should().BeInRange(1, 100);
            }
        }

        [Fact]
        public void BuildDoesNotConstrainIntegerMembersContainingAgeMidWord()
        {
            var context = new BuildContext(new RandomSource(7));

            var sawValueOutsideAgeRange = false;

            for (var index = 0; index < 100 && sawValueOutsideAgeRange == false; index++)
            {
                // "age" appears mid-word in "PageCount"; the Age named source must not hijack it, so the
                // general int source applies and values are not confined to the human age range.
                var actual = context.Build<int>(typeof(Sample), "PageCount");

                if (actual < 1 || actual > 100)
                {
                    sawValueOutsideAgeRange = true;
                }
            }

            sawValueOutsideAgeRange.Should().BeTrue();
        }

        [Theory]
        [InlineData("DateOfBirth")]
        [InlineData("DOB")]
        [InlineData("BirthDate")]
        public void BuildResolvesDateOfBirthWithinHumanPlausibleRange(string memberName)
        {
            var context = new BuildContext(new RandomSource(7));

            for (var index = 0; index < 100; index++)
            {
                var actual = context.Build<DateTime>(typeof(Sample), memberName);

                actual.Kind.Should().Be(DateTimeKind.Utc);
                actual.Year.Should().BeInRange(1919, 2020);
            }
        }

        [Fact]
        public void BuildResolvesDomainNameFromDomainDataRatherThanFullName()
        {
            var context = new BuildContext(new RandomSource(7));

            // "DomainName" contains the "Name" alias of the full-name source, but the more specific Domain
            // source is registered first and "Domain" is a whole word, so it wins.
            var actual = context.Build<string>(typeof(Sample), "DomainName");

            TestData.Domains.Should().Contain(actual);
        }

        private sealed class Sample
        {
        }
    }
}
