namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder.vNext;
    using Xunit;

    public class ModelFacadeTests
    {
        [Fact]
        public void CreateGenericReturnsInstanceFromSlot()
        {
            ModelBuilderSlot<Widget>.Instance = new WidgetBuilder();

            try
            {
                var actual = global::ModelBuilder.vNext.Model.Create<Widget>();

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
            Action action = () => global::ModelBuilder.vNext.Model.Create<Unregistered>();

            action.Should().Throw<ModelBuildException>()
                .Which.FailureKind.Should().Be(FailureKind.NoBuilderForType);
        }

        [Fact]
        public void CreateTypeReturnsInstanceFromRegistry()
        {
            global::ModelBuilder.vNext.Model.Registry.Register(new WidgetBuilder());

            var actual = global::ModelBuilder.vNext.Model.Create(typeof(Widget));

            actual.Should().BeOfType<Widget>();
        }

        [Fact]
        public void CreateTypeThrowsBuildExceptionWhenNoBuilderRegistered()
        {
            Action action = () => global::ModelBuilder.vNext.Model.Create(typeof(Unregistered));

            action.Should().Throw<ModelBuildException>()
                .Which.FailureKind.Should().Be(FailureKind.NoBuilderForType);
        }

        [Fact]
        public void CreateTypeThrowsWithNullType()
        {
            Action action = () => global::ModelBuilder.vNext.Model.Create(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesSlotBuilder()
        {
            ModelBuilderSlot<Widget>.Instance = new WidgetBuilder();

            try
            {
                var instance = new Widget();

                var actual = global::ModelBuilder.vNext.Model.Populate(instance);

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

        private sealed class Widget
        {
            public bool Built { get; set; }
        }

        private sealed class WidgetBuilder : IModelBuilder<Widget>, IModelBuilder
        {
            public Widget Create(BuildContext context, params object?[]? args)
            {
                return Populate(context, new Widget(), args);
            }

            public Widget Populate(BuildContext context, Widget instance, object?[]? args = null)
            {
                instance.Built = true;

                return instance;
            }

            object IModelBuilder.Create(BuildContext context, params object?[]? args)
            {
                return Create(context, args);
            }

            object IModelBuilder.Populate(BuildContext context, object instance, object?[]? args)
            {
                return Populate(context, (Widget)instance, args);
            }

            public Type BuildType => typeof(Widget);
        }
    }
}
