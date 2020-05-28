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
            var actual = Model
                .Mapping<IPrivate, Private>()
                .UsingExecuteStrategy<DefaultExecuteStrategy<IPrivate>>()
                .WriteLog(_output.WriteLine)
                .Create();

            actual.Should().BeOfType<Private>();
        }

        [Fact]
        public void CanMapParameterType()
        {
            var actual = Model
                .UsingExecuteStrategy<DefaultExecuteStrategy<ParameterModel<IItem>>>()
                .WriteLog(_output.WriteLine)
                .Create()!;

            actual.Value.Should().BeOfType<Item>();
        }

        [Fact]
        public void CanMapPropertyType()
        {
            var actual = Model.UsingExecuteStrategy<DefaultExecuteStrategy<PropertyModel<IItem>>>()
                .WriteLog(_output.WriteLine)
                .Create()!;

            actual.Value.Should().BeOfType<Item>();
        }

        [Fact]
        public void CanMapRootType()
        {
            var actual = Model.UsingExecuteStrategy<DefaultExecuteStrategy<IItem>>()
                .WriteLog(_output.WriteLine)
                .Create();

            actual.Should().BeOfType<Item>();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Private : IPrivate
        {
        }
    }
}