namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class StructTests
    {
        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructProperty()
        {
            var actual = Model.Create<StructParent>();

            actual.Child.Id.Should().NotBeEmpty();
            actual.Child.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.Child.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Child.Email.Should().NotBeNullOrWhiteSpace();

            var expression = new Regex("[^a-zA-Z0-9]");

            var expectedFirstName = expression.Replace(actual.Child.FirstName, string.Empty).ToLowerInvariant();
            var expectedLastName = expression.Replace(actual.Child.LastName, string.Empty).ToLowerInvariant();

            actual.Child.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructThatHasNoConstructor()
        {
            var actual = Model.Create<StructModel>();

            actual.Id.Should().NotBeEmpty();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Email.Should().NotBeNullOrWhiteSpace();

            var expression = new Regex("[^a-zA-Z0-9]");

            var expectedFirstName = expression.Replace(actual.FirstName, string.Empty).ToLowerInvariant();
            var expectedLastName = expression.Replace(actual.LastName, string.Empty).ToLowerInvariant();

            actual.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructWithConstructor()
        {
            var key = Guid.NewGuid();
            var value = Model.Create<Person>();

            var actual = Model.Create<KeyValuePair<Guid, Person>>(key, value);

            actual.Key.Should().Be(key);
            actual.Value.Should().Be(value);
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructWithConstructorParameters()
        {
            var actual = Model.Create<KeyValuePair<Guid, Person>>();

            actual.Key.Should().NotBeEmpty();
            actual.Value.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.Value.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Value.PersonalEmail.Should().NotBeNullOrWhiteSpace();

            var expression = new Regex("[^a-zA-Z0-9]");

            var expectedFirstName = expression.Replace(actual.Value.FirstName, string.Empty).ToLowerInvariant();
            var expectedLastName = expression.Replace(actual.Value.LastName, string.Empty).ToLowerInvariant();

            actual.Value.PersonalEmail.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanPopulateStruct()
        {
            var model = new StructModel();

            var actual = Model.Populate(model);

            actual.Id.Should().NotBeEmpty();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Email.Should().NotBeNullOrWhiteSpace();

            var expression = new Regex("[^a-zA-Z0-9]");

            var expectedFirstName = expression.Replace(actual.FirstName, string.Empty).ToLowerInvariant();
            var expectedLastName = expression.Replace(actual.LastName, string.Empty).ToLowerInvariant();

            actual.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        private class StructParent
        {
            public StructModel Child { get; set; }
        }
    }
}