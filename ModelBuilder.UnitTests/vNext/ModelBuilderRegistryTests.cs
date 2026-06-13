namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder.vNext;
    using Xunit;

    public class ModelBuilderRegistryTests
    {
        [Fact]
        public void RegisterThenTryGetReturnsBuilder()
        {
            var sut = new ModelBuilderRegistry();
            var builder = new WidgetBuilder();

            sut.Register(builder);

            sut.TryGet(typeof(Widget), out var actual).Should().BeTrue();
            actual.Should().BeSameAs(builder);
        }

        [Fact]
        public void RegisterThrowsWithNullBuilder()
        {
            var sut = new ModelBuilderRegistry();

            Action action = () => sut.Register(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisteredTypesExposesKeys()
        {
            var sut = new ModelBuilderRegistry();

            sut.Register(new WidgetBuilder());

            sut.RegisteredTypes.Should().Contain(typeof(Widget));
        }

        [Fact]
        public void TryGetReturnsFalseForUnregisteredType()
        {
            var sut = new ModelBuilderRegistry();

            sut.TryGet(typeof(Widget), out var actual).Should().BeFalse();
            actual.Should().BeNull();
        }

        [Fact]
        public void TryGetThrowsWithNullType()
        {
            var sut = new ModelBuilderRegistry();

            Action action = () => sut.TryGet(null!, out _);

            action.Should().Throw<ArgumentNullException>();
        }

        private sealed class Widget
        {
        }

        private sealed class WidgetBuilder : IModelBuilder
        {
            public object Create(BuildContext context, params object?[]? args)
            {
                return new Widget();
            }

            public object Populate(BuildContext context, object instance, object?[]? args = null)
            {
                return instance;
            }

            public Type BuildType => typeof(Widget);
        }
    }
}
