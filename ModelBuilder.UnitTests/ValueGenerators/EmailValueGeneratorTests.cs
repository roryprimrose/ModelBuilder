namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    [SuppressMessage(
        "Microsoft.Globalization",
        "CA1308:NormalizeStringsToUppercase",
        Justification = "Email addresses are lower case by convention.")]
    public class EmailValueGeneratorTests
    {
        [Theory]
        [InlineData("Sam", "De'Vere - Hunt", "sam.deverehunt@")]
        [InlineData("D'Onisi", "Von Tropp", "donisi.vontropp@")]
        [InlineData("Anne-corinne", "O'Carrol", "annecorinne.ocarrol@")]
        [InlineData("D'arcy", "Jones", "darcy.jones")]
        public void GenerateHandlesNonAlphaCharacters(string firstName, string lastName, string expected)
        {
            var person = new Person
            {
                FirstName = firstName,
                LastName = lastName
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsDomainFromDerivedClass()
        {
            var property = typeof(Person).GetProperty(nameof(Person.PersonalEmail));
            var person = new Person();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new MailinatorEmailValueGenerator();

            var actual = (string) sut.Generate(executeStrategy, property);

            actual.Should().EndWith("mailinator.com");
        }

        [Fact]
        public void GenerateReturnsDomainFromTestData()
        {
            var person = new Person();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var domain = actual.Substring(actual.IndexOf("@", StringComparison.Ordinal) + 1);

            TestData.Domains.Any(x => x.ToLowerInvariant() == domain).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsEmailAddressWithNameSpacesRemoved()
        {
            var person = new Person
            {
                FirstName = "De Jour",
                LastName = "Mc Cormick"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var expected = "dejour.mccormick";

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsFirstAndLastNameRelativeToFemaleGender()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var firstName = actual.Substring(0, actual.IndexOf(".", StringComparison.OrdinalIgnoreCase));

            TestData.FemaleNames.Any(x => x.ToLowerInvariant() == firstName).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsFirstAndLastNameRelativeToMaleGender()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var firstName = actual.Substring(0, actual.IndexOf(".", StringComparison.Ordinal));

            TestData.MaleNames.Any(x => x.ToLowerInvariant() == firstName).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingDomainOfContext()
        {
            var parts = new EmailParts
            {
                Domain = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(parts);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            actual.Should().EndWith(parts.Domain);
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstNameAndLastNameOfContext()
        {
            var parts = new EmailParts
            {
                FirstName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                LastName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(parts);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var firstName = EmailValueGenerator.SpecialCharacters.Replace(parts.FirstName, string.Empty);
            var lastName = EmailValueGenerator.SpecialCharacters.Replace(parts.LastName, string.Empty);

            var expected = (firstName + "." + lastName + "@").ToLowerInvariant();

            actual.Should().StartWith(expected);
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstNameLastNameAndDomainOfContext()
        {
            var parts = new EmailParts
            {
                FirstName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                LastName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                Domain = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(parts);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var firstName = EmailValueGenerator.SpecialCharacters.Replace(parts.FirstName, string.Empty);
            var lastName = EmailValueGenerator.SpecialCharacters.Replace(parts.LastName, string.Empty);

            var expected = (firstName + "." + lastName + "@" + parts.Domain).ToLowerInvariant();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstOfContext()
        {
            var person = new Person
            {
                FirstName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var expected = person.FirstName.Substring(0, 1);

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingLastNameOfContext()
        {
            var person = new Person
            {
                LastName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            var expected = person.LastName;

            actual.Should().Contain(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var firstPerson = new Person
            {
                FirstName = "De Jour",
                LastName = "Mc Cormick"
            };
            var firstBuildChain = new BuildHistory();
            var firstExecuteStrategy = Substitute.For<IExecuteStrategy>();

            firstExecuteStrategy.BuildChain.Returns(firstBuildChain);

            firstBuildChain.Push(firstPerson);

            var sut = new Wrapper();

            var first = (string) sut.RunGenerate(typeof(string), "email", firstExecuteStrategy);

            var secondPerson = new Person
            {
                FirstName = "Sam",
                LastName = "Johns"
            };
            var secondBuildChain = new BuildHistory();
            var secondExecuteStrategy = Substitute.For<IExecuteStrategy>();

            secondExecuteStrategy.BuildChain.Returns(secondBuildChain);

            secondBuildChain.Push(secondPerson);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) sut.RunGenerate(typeof(string), "email", secondExecuteStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValue()
        {
            var firstPerson = new Person
            {
                FirstName = "De Jour",
                LastName = "Mc Cormick"
            };
            var firstBuildChain = new BuildHistory();
            var firstExecuteStrategy = Substitute.For<IExecuteStrategy>();

            firstExecuteStrategy.BuildChain.Returns(firstBuildChain);

            firstBuildChain.Push(firstPerson);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "email", firstExecuteStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
            actual.As<string>().Should().Contain("@");
        }

        [Fact]
        public void GenerateReturnsValueWhenContextLacksNameAndGenderProperties()
        {
            var model = new SlimModel();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(model);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "email", executeStrategy);

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGenerator()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "email", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "email", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var person = new Person();
            var buildChain = new BuildHistory();

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        private class Wrapper : EmailValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}