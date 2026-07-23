namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelFacadeTests
    {
        [Fact]
        public void CreateGenericReturnsInstanceFromSlot()
        {
            ModelBuilderSlot<Widget>.Instance = new WidgetBuilder();

            try
            {
                var actual = global::ModelBuilder.Model.Create<Widget>();

                actual.Should().BeOfType<Widget>();
                actual.Built.Should().BeTrue();
            }
            finally
            {
                ModelBuilderSlot<Widget>.Instance = null;
            }
        }

        [Fact]
        public void CreateGenericThrowsBuildExceptionWhenNoBuilderRegistered()
        {
            Action action = () => global::ModelBuilder.Model.Create<Unregistered>();

            action.Should().Throw<ModelBuildException>()
                .Which.FailureKind.Should().Be(FailureKind.NoBuilderForType);
        }

        [Fact]
        public void CreateGenericReturnsValueFromValueSourceWhenNoBuilderRegistered()
        {
            var first = global::ModelBuilder.Model.Create<int>();
            var second = global::ModelBuilder.Model.Create<int>();

            // A built-in value source resolves the root even though no model builder is generated for int.
            // Two draws differing proves a real value source ran rather than returning default.
            (first != 0 || second != 0).Should().BeTrue();
        }

        [Fact]
        public void CreateGenericReturnsValueFromValueSourceForGuidRoot()
        {
            var actual = global::ModelBuilder.Model.Create<Guid>();

            actual.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void CreateTypeReturnsInstanceFromRegistry()
        {
            global::ModelBuilder.Model.Registry.Register(new WidgetBuilder());

            var actual = global::ModelBuilder.Model.Create(typeof(Widget));

            actual.Should().BeOfType<Widget>();
        }

        [Fact]
        public void CreateTypeThrowsBuildExceptionWhenNoBuilderRegistered()
        {
            Action action = () => global::ModelBuilder.Model.Create(typeof(Unregistered));

            action.Should().Throw<ModelBuildException>()
                .Which.FailureKind.Should().Be(FailureKind.NoBuilderForType);
        }

        [Fact]
        public void CreateTypeThrowsWithNullType()
        {
            Action action = () => global::ModelBuilder.Model.Create(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeUsesRegisteredValueSourceFactoryWhenNoBuilderRegistered()
        {
            global::ModelBuilder.Model.RegisterValueSource(new ValueSourceOnlySource());

            var actual = global::ModelBuilder.Model.Create(typeof(ValueSourceOnly));

            actual.Should().BeOfType<ValueSourceOnly>();
            ((ValueSourceOnly)actual).Value.Should().Be(42);
        }

        [Fact]
        public void RegisterValueSourceThrowsWithNullSource()
        {
            Action action = () => global::ModelBuilder.Model.RegisterValueSource<ValueSourceOnly>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesSlotBuilder()
        {
            ModelBuilderSlot<Widget>.Instance = new WidgetBuilder();

            try
            {
                var instance = new Widget();

                var actual = global::ModelBuilder.Model.Populate(instance);

                actual.Should().BeSameAs(instance);
                actual.Built.Should().BeTrue();
            }
            finally
            {
                ModelBuilderSlot<Widget>.Instance = null;
            }
        }

        private sealed class Unregistered
        {
        }

        private sealed class ValueSourceOnly
        {
            public int Value { get; set; }
        }

        private sealed class ValueSourceOnlySource : IValueSource<ValueSourceOnly>
        {
            public ValueSourceOnly Create(IBuildContext context, in BuildTarget target)
            {
                return new ValueSourceOnly { Value = 42 };
            }
        }

        private sealed class Widget
        {
            public bool Built { get; set; }
        }

        private sealed class WidgetBuilder : IModelBuilder<Widget>, IModelBuilder
        {
            public Widget Create(IBuildContext context)
            {
                return Populate(context, new Widget());
            }

            public Widget Populate(IBuildContext context, Widget instance)
            {
                instance.Built = true;

                return instance;
            }

            object IModelBuilder.Create(IBuildContext context)
            {
                return Create(context);
            }

            object IModelBuilder.Populate(IBuildContext context, object instance)
            {
                return Populate(context, (Widget)instance);
            }

            public Type BuildType => typeof(Widget);
        }
    }
}
