namespace ModelBuilder.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class DefaultPropertyResolverTests
    {
        [Fact]
        public void CanPopulateReturnsFalseWhenPropertiesetterIsStatic()
        {
            var propertyInfo = typeof(StaticSetter).GetProperty(nameof(StaticSetter.Person));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenPropertyGetterIsStatic()
        {
            var propertyInfo = typeof(StaticGetter).GetProperty(nameof(StaticGetter.Person));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenPropertyIsPrivate()
        {
            var propertyInfo = typeof(PrivateProp).GetProperty(
                "Person",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty);

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenReadOnlyPropertyIsStringType()
        {
            var propertyInfo = typeof(PrivateString).GetProperty(nameof(PrivateString.Name));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsFalseWhenReadOnlyPropertyIsValueType()
        {
            var propertyInfo = typeof(PrivateValue).GetProperty(nameof(PrivateValue.Id));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void CanPopulateReturnsTrueWhenPropertiesetterIsPublic()
        {
            var propertyInfo = typeof(Address).GetProperty(nameof(Address.AddressLine1));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanPopulateReturnsTrueWhenReadOnlyPropertyIsReferenceType()
        {
            var propertyInfo = typeof(ReadOnlyParent).GetProperty(nameof(ReadOnlyParent.Company));

            var sut = new DefaultPropertyResolver();

            var actual = sut.CanPopulate(propertyInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullPropertyInfo()
        {
            var sut = new DefaultPropertyResolver();

            Action action = () => sut.CanPopulate(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertiesSkipsNullValues()
        {
            var configuration = Model.UsingDefaultConfiguration();
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
        public void ShouldPopulatePropertyReturnsFalseWhenIgnoreRuleMatched()
        {
            var configuration = Model.UsingDefaultConfiguration().AddIgnoreRule<Person>(x => x.Address);
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsFalseWhenParametersMatchPropertyReferenceType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsFalseWhenParametersMatchPropertyValueType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParameterMatchesArgumentList()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParametersMatchPropertyValueType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                instance.First, Guid.NewGuid(), null, int.MinValue, false
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenNoParameterTypesMatchPropertyType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                new Person(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenParametersDoNotMatchPropertyReferenceType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>();
            var args = new object[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsDefaultValueTypeValue()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>().Set(x => x.Id = Guid.Empty);
            var args = new object[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void
            ShouldPopulatePropertyReturnsTrueWhenPropertyContainsNullAndPropertyTypeCannotBeCreatedForEqualityChecking()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<ReadOnlyModelParent>().Set(x => x.Child = null);
            var args = new object[]
            {
                Guid.NewGuid()
            };

            var propertyInfo = typeof(ReadOnlyModelParent).GetProperty(nameof(ReadOnlyModelParent.Child));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsNullReferenceTypeValue()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>().Set(x => x.First = null);
            var args = new object[]
            {
                instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyContainsValueTypeValueNotMatchingConstructor()
        {
            var configuration = Model.UsingDefaultConfiguration();
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
        [SuppressMessage(
            "Microsoft.Design",
            "CA1825",
            Justification = "The Array.Empty<T> is not available on net452.")]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyNotIgnoredAndEmptyArgumentsProvided()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, new object[0]);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyReturnsTrueWhenPropertyNotIgnoredAndNullArgumentsProvided()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            var actual = sut.ShouldPopulateProperty(configuration, instance, propertyInfo, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullConfiguration()
        {
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.ShouldPopulateProperty(null, instance, propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullInstance()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address));

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.ShouldPopulateProperty(configuration, null, propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldPopulatePropertyThrowsExceptionWithNullPropertyInfo()
        {
            var configuration = Model.UsingDefaultConfiguration();
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