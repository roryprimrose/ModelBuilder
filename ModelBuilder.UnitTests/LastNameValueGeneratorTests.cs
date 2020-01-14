namespace ModelBuilder.UnitTests
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using Models;
    using NSubstitute;
    using Xunit;

    public class LastNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsNameTest()
        {
            var person = new PersonWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", executeStrategy);

            TestData.LastNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidatorTest()
        {
            var target = new LastNameValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}