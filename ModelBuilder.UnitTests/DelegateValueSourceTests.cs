namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class DelegateValueSourceTests
    {
        [Fact]
        public void CreateInvokesContextAndTargetFactoryWithBuildContextAndTarget()
        {
            var context = new BuildContext(new RandomSource(1));
            var target = new BuildTarget(typeof(string), "Member");
            IBuildContext? capturedContext = null;
            BuildTarget capturedTarget = default;

            var sut = new DelegateValueSource<int>((ctx, t) =>
            {
                capturedContext = ctx;
                capturedTarget = t;

                return 42;
            });

            var actual = sut.Create(context, target);

            actual.Should().Be(42);
            capturedContext.Should().BeSameAs(context);
            capturedTarget.Should().Be(target);
        }

        [Fact]
        public void CreateInvokesContextOnlyFactoryWithBuildContext()
        {
            var context = new BuildContext(new RandomSource(1));
            var target = new BuildTarget(typeof(string));
            IBuildContext? captured = null;

            var sut = new DelegateValueSource<int>(ctx =>
            {
                captured = ctx;

                return 7;
            });

            var actual = sut.Create(context, target);

            actual.Should().Be(7);
            captured.Should().BeSameAs(context);
        }

        [Fact]
        public void ThrowsWithNullContextAndTargetFactory()
        {
            Action action = () => new DelegateValueSource<int>((Func<IBuildContext, BuildTarget, int>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsWithNullContextOnlyFactory()
        {
            Action action = () => new DelegateValueSource<int>((Func<IBuildContext, int>)null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
