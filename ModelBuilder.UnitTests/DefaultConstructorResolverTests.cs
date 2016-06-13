namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultConstructorResolverTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultConstructorResolverTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ResolveMatchesConstructorWithDerivedParameterTypesTest()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Person), person);

            constructor.GetParameters().Length.Should().Be(1);
        }

        [Fact]
        public void ResolveMatchesConstructorWithMatchingParametersTypesTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(
                typeof(WithValueParameters),
                "first",
                "last",
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount);

            constructor.GetParameters().Length.Should().Be(6);
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersExcludingConstructorsWithSameTypeTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Other));

            constructor.GetParameters().Should().HaveCount(3);
            constructor.GetParameters().Should().NotContain(x => x.ParameterType == typeof(Other));
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithInterfaceAndAbstractParameters));

            constructor.GetParameters().Should().HaveCount(1);
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorOnSimpleModelTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Simple));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorWhenManyConstructorsAvailableTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithMixedValueParameters));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsParameterConstructorTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithValueParameters));

            constructor.GetParameters().Should().NotBeEmpty();
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsConstructorsThatReferenceTheSameTypeTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Clone));

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsCopyConstructorTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Copy));

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoConstructorMatchingSpecifiedParametersTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Simple), Guid.NewGuid().ToString(), true, 123);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoPublicConstructorFoundTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Singleton));

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenParameterValuesDoNotMatchParameterTypesTest()
        {
            var target = new DefaultConstructorResolver();

            var priority = Convert.ToDouble(Environment.TickCount);

            Action action =
                () =>
                    target.Resolve(
                        typeof(WithValueParameters),
                        "first",
                        "last",
                        DateTime.UtcNow,
                        true,
                        Guid.NewGuid(),
                        priority);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        public class Clone
        {
            public Clone(Clone source)
            {
            }

            public Clone(Clone source, string value)
            {
            }

            private Clone()
            {
            }

            public Clone Create()
            {
                return new Clone();
            }
        }

        public class Copy
        {
            public Copy(Copy source)
            {
            }

            private Copy()
            {
            }

            public Copy Create()
            {
                return new Copy();
            }
        }

        public class Other
        {
            public Other(Other source)
            {
            }

            public Other(Other source, string value)
            {
            }

            public Other(Guid id, string value, int priority)
            {
            }

            public Other(Guid id, string value, int priority, Stream data)
            {
            }

            private Other()
            {
            }

            public Other Create()
            {
                return new Other();
            }
        }
    }
}