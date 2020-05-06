namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
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
        public void GetOrderedParametersReturnsEmptyWhenConstructorHasNoParameters()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructor = typeof(Numbers).GetConstructors().First();

            var sut = new DefaultConstructorResolver();

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void GetOrderedParametersReturnsParametersInDeclaredOrderWhenNoExecuteOrderRulesMatch()
        {
            var configuration = new BuildConfiguration();
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            var sut = new DefaultConstructorResolver();

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual[0].Name.Should().Be("email");
            actual[1].Name.Should().Be("domain");
            actual[2].Name.Should().Be("lastName");
            actual[3].Name.Should().Be("firstName");
            actual[4].Name.Should().Be("gender");
        }

        [Fact]
        public void GetOrderedParametersReturnsParametersWhenExecuteOrderRulesIsNull()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            configuration.ExecuteOrderRules.Returns((ICollection<IExecuteOrderRule>) null);

            var sut = new DefaultConstructorResolver();

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual.Should().HaveCount(5);
        }

        [Fact]
        public void GetOrderedParametersReturnsPropertiesInDescendingOrder()
        {
            var configuration = new BuildConfiguration()
                .AddExecuteOrderRule(NameExpression.Gender, 50)
                .AddExecuteOrderRule(NameExpression.FirstName, 40)
                .AddExecuteOrderRule(NameExpression.LastName, 30)
                .AddExecuteOrderRule(NameExpression.Domain, 20)
                .AddExecuteOrderRule(NameExpression.Email, 10);
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            var sut = new DefaultConstructorResolver();

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual[0].Name.Should().Be("gender");
            actual[1].Name.Should().Be("firstName");
            actual[2].Name.Should().Be("lastName");
            actual[3].Name.Should().Be("domain");
            actual[4].Name.Should().Be("email");
        }

        [Fact]
        public void GetOrderedParametersThrowsExceptionWithNullConfiguration()
        {
            var constructor = typeof(Person).GetConstructors().First();

            var sut = new DefaultConstructorResolver();

            Action action = () => sut.GetOrderedParameters(null, constructor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetOrderedParametersThrowsExceptionWithNullConstructor()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultConstructorResolver();

            Action action = () => sut.GetOrderedParameters(configuration, null);

            action.Should().Throw<ArgumentNullException>();
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
        public void ResolveReturnsConstructorForStructThatHasConstructors()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(KeyValuePair<Guid, Person>));

            constructor.Should().NotBeNull();
            constructor.GetParameters().Should().HaveCount(2);
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
        public void ResolveReturnsConstructorWithLeastParameters()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithInterfaceAndAbstractParameters));

            constructor.GetParameters().Should().HaveCount(1);
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
        public void ResolveReturnsNullForStructThatHasNoConstructors()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(StructModel));

            constructor.Should().BeNull();
        }

        [Fact]
        public void ResolveReturnsParameterConstructor()
        {
            var sut = new DefaultConstructorResolver();

            var constructor = sut.Resolve(typeof(WithValueParameters));

            constructor.GetParameters().Should().NotBeEmpty();
        }

        [Fact]
        public void ResolveThrowsExceptionForEnum()
        {
            var sut = new DefaultConstructorResolver();

            Action action = () => sut.Resolve(typeof(Gender));

            _output.WriteLine(action.Should().Throw<MissingMemberException>().And.Message);
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