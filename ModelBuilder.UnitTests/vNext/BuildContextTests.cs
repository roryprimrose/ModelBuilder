namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using ModelBuilder.vNext;
    using Xunit;

    public class BuildContextTests
    {
        [Fact]
        public void BuildPathReflectsEnteredFrames()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterRoot(typeof(Order)))
            {
                using (sut.EnterMember(typeof(Order), "Customer", typeof(Customer)))
                {
                    sut.BuildPath.Should().HaveCount(2);
                    sut.BuildPath[0].MemberType.Should().Be(typeof(Order));
                    sut.BuildPath[1].MemberName.Should().Be("Customer");
                }
            }
        }

        [Fact]
        public void ConstructorThrowsWithNullRandom()
        {
            Action action = () => _ = new BuildContext(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ConstructorUsesNullBuildLogByDefault()
        {
            var sut = new BuildContext(new RandomSource(1));

            sut.Log.Should().BeSameAs(NullBuildLog.Instance);
        }

        [Fact]
        public void CreateBuildExceptionCapturesCurrentTargetAndPath()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterRoot(typeof(Order)))
            using (sut.EnterMember(typeof(Order), "Customer", typeof(Customer)))
            {
                var actual = sut.CreateBuildException("No source", FailureKind.NoValueSource);

                actual.FailureKind.Should().Be(FailureKind.NoValueSource);
                actual.TargetType.Should().Be(typeof(Customer));
                actual.TargetMember.Should().Be("Customer");
                actual.BuildPath.Should().HaveCount(2);
            }
        }

        [Fact]
        public void CurrentTargetIsNullBeforeAnyFrameEntered()
        {
            var sut = new BuildContext(new RandomSource(1));

            sut.CurrentTarget.Should().BeNull();
        }

        [Fact]
        public void DepthReturnsToZeroAfterScopesDisposed()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterRoot(typeof(Order)))
            {
                sut.Depth.Should().Be(1);
            }

            sut.Depth.Should().Be(0);
        }

        [Fact]
        public void IsDepthExceededReturnsTrueAtMaxDepth()
        {
            var options = new BuildContextOptions
            {
                MaxDepth = 1
            };
            var sut = new BuildContext(new RandomSource(1), null, options);

            using (sut.EnterRoot(typeof(Order)))
            {
                sut.IsDepthExceeded.Should().BeTrue();
            }
        }

        [Fact]
        public void IsInBuildChainDetectsAncestorType()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterRoot(typeof(Order)))
            {
                sut.IsInBuildChain(typeof(Order)).Should().BeTrue();
                sut.IsInBuildChain(typeof(Customer)).Should().BeFalse();
            }
        }

        [Fact]
        public void IsInBuildChainThrowsWithNullType()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.IsInBuildChain(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RandomIsExposed()
        {
            var random = new RandomSource(99);

            var sut = new BuildContext(random);

            sut.Random.Should().BeSameAs(random);
        }

        private sealed class Customer
        {
        }

        private sealed class Order
        {
        }
    }
}
