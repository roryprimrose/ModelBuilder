namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class TypeMappingTests
    {
        private readonly ITestOutputHelper _output;

        public TypeMappingTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TypeMappingChangesReturnType()
        {
            var actual = Model.Mapping<ITestItem, TestItem>().WriteLog<ITestItem>(_output.WriteLine).Create();

            actual.Should().BeOfType<TestItem>();
        }
    }
}