namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;
    using Xunit.Abstractions;

    public class StructTests
    {
        private readonly ITestOutputHelper _output;

        public StructTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructProperty()
        {
            var actual = Model.WriteLog<StructParent>(_output.WriteLine).Create();

            actual.Child.Id.Should().NotBeEmpty();
            actual.Child.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.Child.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Child.Email.Should().NotBeNullOrWhiteSpace();

            var expectedFirstName = EmailValueGenerator.SpecialCharacters.Replace(actual.Child.FirstName!, string.Empty)
                .ToLowerInvariant();
            var expectedLastName = EmailValueGenerator.SpecialCharacters.Replace(actual.Child.LastName!, string.Empty)
                .ToLowerInvariant();

            actual.Child.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructThatHasNoConstructor()
        {
            var actual = Model.WriteLog<StructModel>(_output.WriteLine).Create();

            actual.Id.Should().NotBeEmpty();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Email.Should().NotBeNullOrWhiteSpace();

            var expectedFirstName = EmailValueGenerator.SpecialCharacters.Replace(actual.FirstName!, string.Empty)
                .ToLowerInvariant();
            var expectedLastName = EmailValueGenerator.SpecialCharacters.Replace(actual.LastName!, string.Empty)
                .ToLowerInvariant();

            actual.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructWithConstructor()
        {
            var key = Guid.NewGuid();
            var value = Model.WriteLog<Person>(_output.WriteLine).Create();

            var actual = Model.WriteLog<KeyValuePair<Guid, Person>>(_output.WriteLine).Create(key, value);

            actual.Key.Should().Be(key);
            actual.Value.Should().Be(value);
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanCreateStructWithConstructorParameters()
        {
            var actual = Model.WriteLog<KeyValuePair<Guid, Person>>(_output.WriteLine).Create();

            actual.Key.Should().NotBeEmpty();
            actual.Value.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.Value.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Value.PersonalEmail.Should().NotBeNullOrWhiteSpace();

            var expectedFirstName = EmailValueGenerator.SpecialCharacters.Replace(actual.Value.FirstName!, string.Empty)
                .ToLowerInvariant();
            var expectedLastName = EmailValueGenerator.SpecialCharacters.Replace(actual.Value.LastName!, string.Empty)
                .ToLowerInvariant();

            actual.Value.PersonalEmail.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        [Fact]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
            Justification = "Email addresses are lowercase")]
        public void CanPopulateStruct()
        {
            var model = new StructModel();

            var actual = Model.WriteLog<StructModel>(_output.WriteLine).Populate(model);

            actual.Id.Should().NotBeEmpty();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Email.Should().NotBeNullOrWhiteSpace();

            var expectedFirstName = EmailValueGenerator.SpecialCharacters.Replace(actual.FirstName!, string.Empty)
                .ToLowerInvariant();
            var expectedLastName = EmailValueGenerator.SpecialCharacters.Replace(actual.LastName!, string.Empty)
                .ToLowerInvariant();

            actual.Email.Should().StartWith(expectedFirstName + "." + expectedLastName + "@");
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class StructParent
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public StructModel Child { get; set; }
        }
    }
}