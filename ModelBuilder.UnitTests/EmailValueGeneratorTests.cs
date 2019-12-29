namespace ModelBuilder.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    [SuppressMessage(
        "Microsoft.Globalization",
        "CA1308:NormalizeStringsToUppercase",
        Justification = "Email addresses are lower case by convention.")]
    public class EmailValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsDomainFromDerivedClassTest()
        {
            var person = new Person();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new MailinatorEmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            actual.Should().EndWith("mailinator.com");
        }

        [Fact]
        public void GenerateReturnsDomainFromTestDataTest()
        {
            var person = new Person();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var domain = actual.Substring(actual.IndexOf("@", StringComparison.Ordinal) + 1);

            TestData.Domains.Any(x => x.ToLowerInvariant() == domain).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsEmailAddressWithNameSpacesRemovedTest()
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

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var expected = "dejour.mccormick";

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsFirstAndLastNameRelativeToFemaleGenderTest()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var firstName = actual.Substring(0, actual.IndexOf(".", StringComparison.OrdinalIgnoreCase));

            TestData.FemaleNames.Any(x => x.ToLowerInvariant() == firstName).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsFirstAndLastNameRelativeToMaleGenderTest()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var firstName = actual.Substring(0, actual.IndexOf(".", StringComparison.Ordinal));

            TestData.MaleNames.Any(x => x.ToLowerInvariant() == firstName).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingDomainOfContextTest()
        {
            var parts = new EmailParts
            {
                Domain = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(parts);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            actual.Should().EndWith(parts.Domain);
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstNameAndLastNameOfContextTest()
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

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var expected = parts.FirstName + "." + parts.LastName;

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstNameLastNameAndDomainOfContextTest()
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

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var expected = parts.FirstName + "." + parts.LastName + "@" + parts.Domain;

            actual.Should().Be(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingFirstOfContextTest()
        {
            var person = new Person
            {
                FirstName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var expected = person.FirstName.Substring(0, 1);

            actual.Should().StartWith(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomEmailAddressUsingLastNameOfContextTest()
        {
            var person = new Person
            {
                LastName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            var expected = person.LastName;

            actual.Should().Contain(expected.ToLowerInvariant());
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
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

            var target = new EmailValueGenerator();

            var first = (string)target.Generate(typeof(string), "email", firstExecuteStrategy);

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
                second = (string)target.Generate(typeof(string), "email", secondExecuteStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
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

            var target = new EmailValueGenerator();

            var actual = target.Generate(typeof(string), "email", firstExecuteStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
            actual.As<string>().Should().Contain("@");
        }

        [Fact]
        public void GenerateReturnsValueWhenContextLacksNameAndGenderPropertiesTest()
        {
            var model = new SlimModel();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(model);

            var target = new EmailValueGenerator();

            var actual = (string)target.Generate(typeof(string), "email", executeStrategy);

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(Stream), "email")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "")]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EmailValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new EmailValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "email", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "email", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var person = new Person();
            var buildChain = new BuildHistory();

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullContextTest()
        {
            var target = new EmailValueGenerator();

            var actual = target.IsSupported(typeof(string), "email", null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var person = new Person();
            var buildChain = new BuildHistory();

            buildChain.Push(person);

            var target = new EmailValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}