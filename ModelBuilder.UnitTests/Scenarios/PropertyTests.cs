namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PropertyTests
    {
        [Fact]
        public void DoesNotPopulatePropertyContainingConstructorValue()
        {
            var model = Model.Create<SlimModel>();

            var actual = Model.Create<SimpleConstructor>(model);

            actual.Model.Should().Be(model);
        }
    }
}