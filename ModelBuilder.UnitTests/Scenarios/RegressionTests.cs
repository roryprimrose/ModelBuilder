namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class RegressionTests
    {
        private readonly ITestOutputHelper _output;

        public RegressionTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void CreateCanPopulateByteProperty()
        {
            // Validates https://github.com/roryprimrose/ModelBuilder/issues/168 and found not to be an issue in the current code
            var actual = Model.WriteLog<PropertyModel<byte>>(_output.WriteLine).Create();

            _output.WriteLine(actual.Value.ToString());
        }

        [Fact]
        public void CreateShouldSkipSelfReferencingPropertiesOnPopulateException()
        {
            // Fixes https://github.com/roryprimrose/ModelBuilder/issues/347
            var action = () => Model.WriteLog<SelfReferenceInstance>(_output.WriteLine).Create();

            var actual = action.Should().NotThrow().Subject;

            actual.Instance.Should().BeSameAs(actual);
        }
    }
}