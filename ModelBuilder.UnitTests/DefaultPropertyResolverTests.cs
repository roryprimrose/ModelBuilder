namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class DefaultPropertyResolverTests
    {
        [Fact]
        public void CanPopulateReturnsFalseWhenPropertyGetterIsStaticTest()
        {
            var propertyInfo = typeof(StaticGetter).GetProperty(nameof(StaticGetter.Person));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenPropertyIsPrivateTest()
        {
            var propertyInfo = typeof(PrivateProp).GetProperty("Person",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty);

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenPropertySetterIsStaticTest()
        {
            var propertyInfo = typeof(StaticSetter).GetProperty(nameof(StaticSetter.Person));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenReadOnlyPropertyIsStringTypeTest()
        {
            var propertyInfo = typeof(PrivateString).GetProperty(nameof(PrivateString.Name));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenReadOnlyPropertyIsValueTypeTest()
        {
            var propertyInfo = typeof(PrivateValue).GetProperty(nameof(PrivateValue.Id));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsTrueWhenPropertySetterIsPublicTest()
        {
            var propertyInfo = typeof(Address).GetProperty(nameof(Address.AddressLine1));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanPopulateReturnsTrueWhenReadOnlyPropertyIsReferenceTypeTest()
        {
            var propertyInfo = typeof(ReadOnlyParent).GetProperty(nameof(ReadOnlyParent.Company));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullPropertyInfoTest()
        {
            var sut = new DefaultPropertyResolver();

            Action action = () => sut.CanPopulate(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsFalseWhenIgnoreRuleMatchedTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().AddIgnoreRule<Person>(x => x.Address).Compile();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsFalseWhenParametersMatchPropertyReferenceTypeTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsFalseWhenParametersMatchPropertyValueTypeTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParameterMatchesArgumentListTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParametersMatchPropertyValueTypeTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {instance.First, Guid.NewGuid(), null, int.MinValue, false};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParameterTypesMatchPropertyTypeTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {new Person(), instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenParametersDoNotMatchPropertyReferenceTypeTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[] {new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsDefaultValueTypeValueTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>().Set(x => x.Id = Guid.Empty);
            var args = new object[] {new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void
            ShouldPopulatePropertyReturnsTrueWhenPropertyContainsNullAndPropertyTypeCannotBeCreatedForEqualityCheckingTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<ReadOnlyModelParent>().Set(x => x.Child = null);
            var args = new object[] {Guid.NewGuid()};

            var propertyInfo = typeof(ReadOnlyModelParent).GetProperty(nameof(ReadOnlyModelParent.Child));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsNullReferenceTypeValueTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>().Set(x => x.First = null);
            var args = new object[] {instance.Id, instance.RefNumber, instance.Number, instance.Value};

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsValueTypeValueNotMatchingConstructorTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                new Company(), Guid.NewGuid(), instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyNotIgnoredAndEmptyArgumentsProvidedTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, new object[0]);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyNotIgnoredAndNullArgumentsProvidedTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertySkipsNullValuesTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                null, new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullConfigurationTest()
        {
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.ShouldPopulateProperty(null, instance, propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullInstanceTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.ShouldPopulateProperty(configuration, null, propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullPropertyInfoTest()
        {
            var configuration = new DefaultBuildStrategyCompiler().Compile();
            var instance = new Person();

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.ShouldPopulateProperty(configuration, instance, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        private class PrivateProp
        {
            private Person Person { set; get; }
        }

        private class PrivateString
        {
            public string Name { get; }
        }

        private class PrivateValue
        {
            public Guid Id { get; }
        }

        private class ReadOnlyModelParent
        {
            public ReadOnlyModel Child { get; set; }
        }

        private class StaticGetter
        {
            public static Person Person { get; }
        }

        private class StaticSetter
        {
            public static Person Person { set; get; }
        }
    }
}