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
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(Person), (Person) null);

            constructor.GetParameters().Length.Should().Be(1);
        }

        [Fact]
        public void ResolveMatchesConstructorWithDerivedParameterTypes()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(Person), person);

            constructor.GetParameters().Length.Should().Be(1);
        }

        [Fact]
        public void ResolveMatchesConstructorWithMatchingParametersTypes()
        {
            var args = new object[]
            {
                "first", "last", DateTime.UtcNow, true, Guid.NewGuid(), Environment.TickCount
            };

            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithValueParameters), args);

            constructor.GetParameters().Length.Should().Be(6);
        }

        [Fact]
        public void ResolveReturnsConstructorThatMatchesAgainstNullParameters()
        {
            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(Other), Guid.NewGuid(), null, 123, null);

            actual.GetParameters().Length.Should().Be(4);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullableParameterWithNullValue()
        {
            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(WithConstructorParameters), null, Guid.NewGuid(), null, 123, true);

            actual.GetParameters().Length.Should().Be(5);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndArgCountFillsNonOptionalParam()
        {
            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(Optionals), null, 10, "third");

            actual.GetParameters().Length.Should().Be(5);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndMatchesDerivedType()
        {
            var company = new SpecificCompany();

            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(Derived), null, company);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNonNullArgIsCopyConstructor()
        {
            var source = Clone.Create();

            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(Clone), source, null);

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWhenArgsContainsNullAndNullArgIsCopyConstructor()
        {
            var sut = new DefaultConstructorResolver();

            var actual = sut.Resolve(typeof(Clone), null, "second");

            actual.GetParameters().Length.Should().Be(2);
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersExcludingConstructorsWithSameType()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(Other));

            constructor.GetParameters().Should().HaveCount(3);
            constructor.GetParameters().Should().NotContain(x => x.ParameterType == typeof(Other));
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParameters()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithInterfaceAndAbstractParameters));

            constructor.GetParameters().Should().HaveCount(1);
        }

        [Fact]
        public void ResolveReturnsConstructorWithLeastParametersWhenArgsIsNull()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithInterfaceAndAbstractParameters), null);

            constructor.GetParameters().Should().HaveCount(1);
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorOnSimpleModel()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(Simple));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsDefaultConstructorWhenManyConstructorsAvailable()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithMixedValueParameters));

            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void ResolveReturnsParameterConstructor()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithValueParameters));

            constructor.GetParameters().Should().NotBeEmpty();
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndArgCountLessThanOptionalParam()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Optionals), "first", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndHasArgParamTypeMismatch()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Optionals), "first", "stuff", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndNoMatchFoundOnTypes()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Other), Guid.NewGuid(), null, "NotAMatchingParameter", null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndParamIsSubtype()
        {
            var company = new Company();

            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Derived), null, company, "third");

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullAndTooManyArgumentsProvided()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(
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
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Other), Guid.NewGuid(), "NextParameterIsInt", null, null);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenArgsContainsNullNoOptionalParamsAndArgumentCountMismatch()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(WithMixedValueParameters), "first", null, "this doesn't exist");

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsConstructorsThatReferenceTheSameType()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Clone));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenClassOnlyContainsCopyConstructor()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Copy));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoConstructorMatchingSpecifiedParameters()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Simple), Guid.NewGuid().ToString(), true, 123);

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenNoPublicConstructorFound()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Singleton));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWhenParameterValuesDoNotMatchParameterTypes()
        {
            var sut = new DefaultConstructorResolver();

            var priority = Convert.ToDouble(Environment.TickCount);

            Action action = () => sut.Resolve(
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
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Singleton));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullType()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}