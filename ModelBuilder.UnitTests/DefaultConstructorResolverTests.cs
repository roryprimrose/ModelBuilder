namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
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
        public void ResolveMatchesConstructorMatchingParameterLengthWhereParameterValueIsNull()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Person), (Person) null);

            constructor.GetParameters().Length.Should().Be(1);
        }

        [Fact]
        public void ResolveMatchesConstructorWithDerivedParameterTypes()
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
        public void ResolveMatchesConstructorWithMatchingParametersTypes()
        {
            var args = new object[]
            {
                "first", "last", DateTime.UtcNow, true, Guid.NewGuid(), Environment.TickCount
            };

            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithValueParameters), args);

            constructor.GetParameters().Length.Should().Be(6);
        }

        [Fact]
        public void ResolveReturnsConstructorThatMatchesAgainstNullParameters()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Other), Guid.NewGuid(), null, 123, null);

            actual.GetParameters().Length.Should().Be(4);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullableParameterWithNullValue()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(WithConstructorParameters), null, Guid.NewGuid(), null, 123, true);

            actual.GetParameters().Length.Should().Be(5);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndArgCountFillsNonOptionalParam()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Optionals), null, 10, "third");

            actual.GetParameters().Length.Should().Be(5);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndMatchesDerivedType()
        {
            var company = new SpecificCompany();

            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Derived), null, company);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNonNullArgIsCopyConstructor()
        {
            var source = Clone.Create();

            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Clone), source, null);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNullArgIsCopyConstructor()
        {
            var target = new DefaultConstructorResolver();

            var actual = target.Resolve(typeof(Clone), null, "second");

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersExcludingConstructorsWithSameType()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Other));

            constructor.GetParameters().Should().HaveCount(3);
            constructor.GetParameters().Should().NotContain(x => x.ParameterType == typeof(Other));
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParameters()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithInterfaceAndAbstractParameters));

            constructor.GetParameters().Should().HaveCount(1);
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersWhenArgsIsNull()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithInterfaceAndAbstractParameters), null);

            constructor.GetParameters().Should().HaveCount(1);
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorOnSimpleModel()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(Simple));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorWhenManyConstructorsAvailable()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithMixedValueParameters));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsParameterConstructor()
        {
            var target = new DefaultConstructorResolver();

            var constructor = target.Resolve(typeof(WithValueParameters));

            constructor.GetParameters().Should().NotBeEmpty();
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndArgCountLessThanOptionalParam()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Optionals), "first", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndHasArgParamTypeMismatch()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Optionals), "first", "stuff", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndNoMatchFoundOnTypes()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Other), Guid.NewGuid(), null, "NotAMatchingParameter", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndParamIsSubtype()
        {
            var company = new Company();

            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Derived), null, company, "third");

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndTooManyArgumentsProvided()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(
                typeof(Other),
                Guid.NewGuid(),
                null,
                123,
                null,
                "ThisParamDoesn'tMatch");

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullMatchingValueTypeParameter()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Other), Guid.NewGuid(), "NextParameterIsInt", null, null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullNoOptionalParamsAndArgumentCountMismatch()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(WithMixedValueParameters), "first", null, "this doesn't exist");

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsConstructorsThatReferenceTheSameType()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Clone));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsCopyConstructor()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Copy));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoConstructorMatchingSpecifiedParameters()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Simple), Guid.NewGuid().ToString(), true, 123);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoPublicConstructorFound()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Singleton));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenParameterValuesDoNotMatchParameterTypes()
        {
            var target = new DefaultConstructorResolver();

            var priority = Convert.ToDouble(Environment.TickCount);

            Action action = () => target.Resolve(
                typeof(WithValueParameters),
                "first",
                "last",
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                priority);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenWhenOnlyPrivateConstructorAvailable()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(typeof(Singleton));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullType()
        {
            var target = new DefaultConstructorResolver();

            Action action = () => target.Resolve(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}