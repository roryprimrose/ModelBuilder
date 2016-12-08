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
        public void ResolveMatchesConstructorMatchingParameterLengthWhereParameterValueIsNullTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Person), (Person)null);

            constructor.GetParameters().Length.Should().Be(1);
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
            var args = new object[]
            {
                "first",
                "last",
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount
            };

            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithValueParameters), args);

            constructor.GetParameters().Length.Should().Be(6);
        }

        [Fact]
        public void ResolveReturnsConstructorThatMatchesAgainstNullParametersTest()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Other), Guid.NewGuid(), null, 123, null);

            actual.GetParameters().Length.Should().Be(4);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndArgCountFillsNonOptionalParamTest()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Optionals), null, 10, "third");

            actual.GetParameters().Length.Should().Be(5);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndMatchesDerivedTypeTest()
        {
            var company = new SpecificCompany();

            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Derived), null, company);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNonNullArgIsCopyConstructorTest()
        {
            var source = Clone.Create();

            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Clone), source, null);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNullArgIsCopyConstructorTest()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Clone), null, "second");

            actual.GetParameters().Length.Should().Be(2);
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
        public void ResolveReturnsConstructorWithLeastParametersWhenArgsIsNullTest()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithInterfaceAndAbstractParameters), null);

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
        public void ResolveThrowsExceptionWhenArgsContainsNullAndArgCountLessThanOptionalParamTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Optionals), "first", null);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndHasArgParamTypeMismatchTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Optionals), "first", "stuff", null);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndNoMatchFoundOnTypesTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Other), Guid.NewGuid(), null, "NotAMatchingParameter", null);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndParamIsSubtypeTest()
        {
            var company = new Company();

            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Derived), null, company, "third");

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndTooManyArgumentsProvidedTest()
        {
            var target = new DefaultConstructorResolver();

            Action action =
                () => target.Resolve(typeof(Other), Guid.NewGuid(), null, 123, null, "ThisParamDoesn'tMatch");

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullMatchingValueTypeParameterTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Other), Guid.NewGuid(), "NextParameterIsInt", null, null);

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullNoOptionalParamsAndArgumentCountMismatchTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(WithMixedValueParameters), "first", null, "this doesn't exist");

            _output.WriteLine(action.ShouldThrow<MissingMemberException>().And.Message);
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
        public void ResolveThrowsExceptionWhenWhenOnlyPrivateConstructorAvailableTest()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Singleton));

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

            public static Clone Create()
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

        public class Derived
        {
            public Derived(string first, Company second)
            {
            }

            public Derived(string first, SpecificCompany second, string third)
            {
            }
        }

        public class Optionals
        {
            public Optionals(string first)
            {
            }

            public Optionals(string first, int second, string third, int fourth = 0, byte fifth = 4)
            {
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