namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class ConstructorTests
    {
        private readonly ITestOutputHelper _output;

        public ConstructorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoPublicConstructorFound()
        {
            Action action = () => Model.Create<FactoryClass>();

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateReturnsTypeThatDefinesCopyConstructor()
        {
            var actual = Model.WriteLog<Other>(_output.WriteLine).Create();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void DoesNotSetInstanceParameterAssignedToProperty()
        {
            var company = Model.Create<Company>()!;
            var id = Model.Create<Guid>();
            var refNumber = Model.Create<int>();
            var number = Model.Create<int>();
            var value = Model.Create<bool>();

            var model = Model.WriteLog<WithConstructorParameters>(_output.WriteLine)
                .Create(company, id, refNumber, number, value)!;

            model.First.Should().BeSameAs(company);
            model.Second.Should().NotBeSameAs(company);
        }

        [Fact]
        public void PopulatesInstanceTypePropertyWhenConstructorParameterMatchesDefaultTypeValue()
        {
            var id = Model.Create<Guid>();
            var refNumber = Model.Create<int>();
            var number = Model.Create<int>();
            var value = Model.Create<bool>();

            var model = Model.WriteLog<WithConstructorParameters>(_output.WriteLine)
                .Create((Company) null!, id, refNumber, number, value)!;

            model.First.Should().NotBeNull();
        }

        [Fact]
        public void PopulatesPropertyWhenTypeNotMatchingAnyConstructorParameter()
        {
            var company = Model.Create<Company>();
            var id = Model.Create<Guid>();
            var refNumber = Model.Create<int>();
            var number = Model.Create<int>();
            var value = Model.Create<bool>();

            var model = Model.WriteLog<WithConstructorParameters>(_output.WriteLine)
                .Create(company, id, refNumber, number, value)!;

            model.Customer.Should().NotBeNull();
        }

        [Fact]
        public void PopulatesValueTypePropertyWhenConstructorParameterMatchesDefaultTypeValue()
        {
            var company = Model.Create<Company>();
            var id = Guid.Empty;
            var refNumber = Model.Create<int>();
            var number = Model.Create<int>();
            var value = Model.Create<bool>();

            var model = Model.WriteLog<WithConstructorParameters>(_output.WriteLine)
                .Create(company, id, refNumber, number, value)!;

            model.Id.Should().NotBeEmpty();
        }

        private class FactoryClass
        {
            private FactoryClass(Guid value)
            {
                Value = value;
            }

            public static FactoryClass Create(Guid value)
            {
                return new FactoryClass(value);
            }

            public Guid Value { get; }
        }
    }
}