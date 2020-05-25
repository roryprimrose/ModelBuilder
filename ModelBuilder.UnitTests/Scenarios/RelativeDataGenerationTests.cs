namespace ModelBuilder.UnitTests.Scenarios
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class RelativeDataGenerationTests
    {
        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Emails are lower-case by convention")]
        public void AssignsPropertyValuesRelativeToOtherPropertyValues()
        {
            var actual = Model.Create<Person>()!;

            var firstName = EmailValueGenerator.SpecialCharacters.Replace(actual.FirstName, string.Empty);
            var lastName = EmailValueGenerator.SpecialCharacters.Replace(actual.LastName, string.Empty);

            var expected = (firstName + "." + lastName + "@").ToLowerInvariant();

            actual.PersonalEmail.Should()
                .StartWith(expected);
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Emails are lower-case by convention")]
        public void CreatesParameterValuesRelativeToOtherParameterValues()
        {
            var actual = Model.Create<OrderedConstructorParameters>()!;

            if (actual.Gender == Gender.Female)
            {
                TestData.FemaleNames.Should().Contain(actual.FirstName);
            }
            else if (actual.Gender == Gender.Male)
            {
                TestData.MaleNames.Should().Contain(actual.FirstName);
            }

            var firstName = EmailValueGenerator.SpecialCharacters.Replace(actual.FirstName, string.Empty);
            var lastName = EmailValueGenerator.SpecialCharacters.Replace(actual.LastName, string.Empty);

            var expectedEmail = (firstName + "." + lastName + "@" + actual.Domain).ToLowerInvariant();

            actual.Email.Should().StartWith(expectedEmail);
        }
    }
}