namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RelativeDataGenerationTests
    {
        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Emails are lower-case by convention")]
        public void AssignsPropertyValuesRelativeToOtherPropertyValues()
        {
            var actual = Model.Create<Person>();

            var expected = (actual.FirstName + "." + actual.LastName + "@").ToLowerInvariant();

            actual.PersonalEmail.Should()
                .StartWith(expected);
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Emails are lower-case by convention")]
        public void CreatesParameterValuesRelativeToOtherParameterValues()
        {
            var actual = Model.Create<OrderedConstructorParameters>();

            if (actual.Gender == Gender.Female)
            {
                TestData.FemaleNames.Should().Contain(actual.FirstName);
            }
            else if (actual.Gender == Gender.Male)
            {
                TestData.MaleNames.Should().Contain(actual.FirstName);
            }

            var expectedEmail = (actual.FirstName + "." + actual.LastName + "@" + actual.Domain).ToLowerInvariant().Replace(" ", "", StringComparison.CurrentCulture);
            
            actual.Email.Should().StartWith(expectedEmail);
        }
    }
}