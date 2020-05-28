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

        private interface IPrivate
        {
        }

        [Fact]
        public void CanExplicitlyMapPrivateType()
        {
            var strategy = Model.Mapping<IPrivate, Private>().UsingExecuteStrategy<DefaultExecuteStrategy<IPrivate>>();

            var actual = strategy.Create();

            _output.WriteLine(strategy.Log.Output);

            actual.Should().BeOfType<Private>();
        }

        [Fact]
        public void CanMapParameterType()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<ParameterModel<IItem>>>();

            var actual = strategy.Create()!;

            _output.WriteLine(strategy.Log.Output);

            actual.Value.Should().BeOfType<Item>();
        }

        [Fact]
        public void CanMapPropertyType()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<PropertyModel<IItem>>>();

            var actual = strategy.Create()!;

            _output.WriteLine(strategy.Log.Output);

            actual.Value.Should().BeOfType<Item>();
        }

        [Fact]
        public void CanMapRootType()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<IItem>>();

            var actual = strategy.Create();

            _output.WriteLine(strategy.Log.Output);

            actual.Should().BeOfType<Item>();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Private : IPrivate
        {
        }
    }
}