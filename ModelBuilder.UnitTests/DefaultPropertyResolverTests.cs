﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultPropertyResolverTests
    {
        [Theory]
        [InlineData(CacheLevel.Global)]
        [InlineData(CacheLevel.PerInstance)]
        [InlineData(CacheLevel.None)]
        public void CacheLevelReturnsConstructorValue(CacheLevel cacheLevel)
        {
            var sut = new DefaultPropertyResolver(cacheLevel);

            sut.CacheLevel.Should().Be(cacheLevel);
        }

        [Fact]
        public void GetOrderedPropertiesDoesNotReturnPrivateProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(PrivateProp);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().NotContain(x => x.Name == "Person");
        }

        [Fact]
        public void GetOrderedPropertiesDoesNotReturnReadOnlyStringProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(PrivateString);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().NotContain(x => x.Name == nameof(PrivateString.Name));
        }

        [Fact]
        public void GetOrderedPropertiesDoesNotReturnsReadOnlyValueTypeProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(PrivateValue);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().NotContain(x => x.Name == nameof(PrivateValue.Id));
        }

        [Fact]
        public void GetOrderedPropertiesDoesNotReturnStaticGetterProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(StaticGetter);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().NotContain(x => x.Name == nameof(StaticGetter.Person));
        }

        [Fact]
        public void GetOrderedPropertiesDoesNotReturnStaticSetterProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(StaticSetter);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().NotContain(x => x.Name == nameof(StaticSetter.Person));
        }

        [Theory]
        [InlineData(CacheLevel.Global)]
        [InlineData(CacheLevel.PerInstance)]
        public void GetOrderedPropertiesReturnsCachedValueWhenCacheEnabled(CacheLevel cacheLevel)
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(SlimModel);

            var sut = new DefaultPropertyResolver(cacheLevel);

            var first = sut.GetOrderedProperties(configuration, type);
            var second = sut.GetOrderedProperties(configuration, type);

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void GetOrderedPropertiesReturnsNewInstanceWhenCacheDisabled()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(Person);

            var sut = new DefaultPropertyResolver(CacheLevel.None);

            var first = sut.GetOrderedProperties(configuration, type).ToList();
            var second = sut.GetOrderedProperties(configuration, type).ToList();

            first.Should().NotBeSameAs(second);
            first.Should().BeEquivalentTo(second);
        }

        [Fact]
        public void GetOrderedPropertiesReturnsPropertiesInDescendingOrder()
        {
            var configuration = new BuildConfiguration()
                .AddExecuteOrderRule<EmailParts>(x => x.FirstName, 40)
                .AddExecuteOrderRule<EmailParts>(x => x.LastName, 30)
                .AddExecuteOrderRule<EmailParts>(x => x.Domain, 20)
                .AddExecuteOrderRule<EmailParts>(x => x.Email, 10);
            var type = typeof(EmailParts);

            var sut = new DefaultPropertyResolver(CacheLevel.None);

            var actual = sut.GetOrderedProperties(configuration, type).ToList();

            actual[0].Name.Should().Be(nameof(EmailParts.FirstName));
            actual[1].Name.Should().Be(nameof(EmailParts.LastName));
            actual[2].Name.Should().Be(nameof(EmailParts.Domain));
            actual[3].Name.Should().Be(nameof(EmailParts.Email));
        }

        [Fact]
        public void GetOrderedPropertiesReturnsPropertiesWhenExecuteOrderRulesIsNull()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(EmailParts);

            configuration.ExecuteOrderRules.Returns((ICollection<IExecuteOrderRule>)null!);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type).ToList();

            actual.Should().HaveCount(4);
        }

        [Fact]
        public void GetOrderedPropertiesReturnsPublicSetterProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(Address);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().Contain(x => x.Name == nameof(Address.AddressLine1));
        }

        [Fact]
        public void GetOrderedPropertiesReturnsReadOnlyReferenceTypeProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var type = typeof(ReadOnlyParent);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedProperties(configuration, type);

            actual.Should().Contain(x => x.Name == nameof(ReadOnlyParent.Company));
        }

        [Fact]
        public void GetOrderedPropertiesThrowsExceptionWithNullConfiguration()
        {
            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.GetOrderedProperties(null!, typeof(Person));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetOrderedPropertiesThrowsExceptionWithNullTargetType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.GetOrderedProperties(configuration, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenIgnoreRulesAreEmpty()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, null!);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenMatchOnConstructorArgumentValueButNotName()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructorResolver = Substitute.For<IConstructorResolver>();

            var value = Guid.NewGuid().ToString();
            var instance = new Optionals(value);
            var propertyInfo = instance.GetType().GetProperty(nameof(Optionals.Value))!;
            var args = new object?[]
            {
                value
            };
            var constructor = instance.GetType().GetConstructor(new[] { typeof(string) });

            configuration.ConstructorResolver.Returns(constructorResolver);
            constructorResolver.Resolve(typeof(Optionals), args).Returns(constructor);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenNoMatchOnConstructorArguments()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructorResolver = Substitute.For<IConstructorResolver>();

            var instance = new Optionals(Guid.NewGuid().ToString())
            {
                Value = Guid.NewGuid().ToString()
            };
            var propertyInfo = instance.GetType().GetProperty(nameof(Optionals.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString()
            };
            var constructor = instance.GetType().GetConstructor(new[] { typeof(string) });

            configuration.ConstructorResolver.Returns(constructorResolver);
            constructorResolver.Resolve(typeof(Optionals), args).Returns(constructor);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchReferencePropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = new Person();
            var model = new ClassMatchingNameWrapper<Person>(value);
            var propertyInfo =
                typeof(ClassMatchingNameWrapper<Person>).GetProperty(nameof(ClassMatchingNameWrapper<Person>.Value))!;
            var args = new object?[]
            {
                new Person()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchReferencePropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = new Person();
            var model = new ClassMismatchingNameWrapper<Person>(item);
            var propertyInfo =
                typeof(ClassMismatchingNameWrapper<Person>).GetProperty(
                    nameof(ClassMismatchingNameWrapper<Person>.Value))!;
            var args = new object?[]
            {
                new Person()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchStringPropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = Guid.NewGuid().ToString();
            var model = new ClassMatchingNameWrapper<string>(value);
            var propertyInfo =
                typeof(ClassMatchingNameWrapper<string>).GetProperty(nameof(ClassMatchingNameWrapper<string>.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchStringPropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = Guid.NewGuid().ToString();
            var model = new ClassMismatchingNameWrapper<string>(item);
            var propertyInfo =
                typeof(ClassMismatchingNameWrapper<string>).GetProperty(
                    nameof(ClassMismatchingNameWrapper<string>.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchStructPropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = new StructModel { FirstName = Guid.NewGuid().ToString() };
            var model = new StructMatchingNameWrapper<StructModel>(value);
            var propertyInfo =
                typeof(StructMatchingNameWrapper<StructModel>).GetProperty(nameof(StructMatchingNameWrapper<StructModel>
                    .Value))!;
            var args = new object?[]
            {
                new StructModel { FirstName = Guid.NewGuid().ToString() }
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchStructPropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = new StructModel { FirstName = Guid.NewGuid().ToString() };
            var model = new StructMismatchingNameWrapper<StructModel>(item);
            var propertyInfo =
                typeof(StructMismatchingNameWrapper<StructModel>).GetProperty(
                    nameof(StructMismatchingNameWrapper<StructModel>.Value))!;
            var args = new object?[]
            {
                new StructModel { FirstName = Guid.NewGuid().ToString() }
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchValueTypePropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = Environment.TickCount;
            var model = new StructMatchingNameWrapper<int>(value);
            var propertyInfo =
                typeof(StructMatchingNameWrapper<int>).GetProperty(nameof(StructMatchingNameWrapper<int>.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterDoesNotMatchValueTypePropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = Environment.TickCount;
            var model = new StructMismatchingNameWrapper<int>(item);
            var propertyInfo =
                typeof(StructMismatchingNameWrapper<int>).GetProperty(nameof(StructMismatchingNameWrapper<int>.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterMatchesStringPropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = Guid.NewGuid().ToString();
            var model = new ClassMismatchingNameWrapper<string>(item);
            var propertyInfo =
                typeof(ClassMismatchingNameWrapper<string>).GetProperty(
                    nameof(ClassMismatchingNameWrapper<string>.Value))!;
            var args = new object?[]
            {
                item
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterMatchesStructPropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = new StructModel { FirstName = Guid.NewGuid().ToString() };
            var model = new StructMismatchingNameWrapper<StructModel>(item);
            var propertyInfo =
                typeof(StructMismatchingNameWrapper<StructModel>).GetProperty(
                    nameof(StructMismatchingNameWrapper<StructModel>.Value))!;
            var args = new object?[]
            {
                item
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterMatchesValueTypePropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = Environment.TickCount;
            var model = new StructMismatchingNameWrapper<int>(item);
            var propertyInfo =
                typeof(StructMismatchingNameWrapper<int>).GetProperty(nameof(StructMismatchingNameWrapper<int>.Value))!;
            var args = new object?[]
            {
                item
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParametersDoNotMatchArgumentList()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParametersDoNotMatchPropertyReferenceType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParametersDoNotMatchPropertyValueType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                instance.First, Guid.NewGuid(), null!, int.MinValue, false
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenParameterTypesDoNotMatchPropertyType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                new Person(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenPropertyContainsDefaultValueTypeValue()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!.Set(x => x.Id = Guid.Empty);
            var args = new object?[]
            {
                new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void
            IsIgnoredReturnsFalseWhenPropertyContainsNullAndPropertyTypeCannotBeCreatedForEqualityChecking()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<ReadOnlyModelParent>()!.Set(x => x.Child = null!);
            var args = new object?[]
            {
                Guid.NewGuid()
            };

            var propertyInfo = typeof(ReadOnlyModelParent).GetProperty(nameof(ReadOnlyModelParent.Child))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenPropertyContainsNullReferenceTypeValue()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!.Set(x => x.First = null!);
            var args = new object?[]
            {
                instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenPropertyContainsValueTypeValueNotMatchingConstructor()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                new Company(), Guid.NewGuid(), instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Id))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1825",
            Justification = "The Array.Empty<T> is not available on net452.")]
        public void IsIgnoredReturnsFalseWhenPropertyNotIgnoredAndEmptyArgumentsProvided()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, new object[0]);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenPropertyNotIgnoredAndNullArgumentsProvided()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, null!);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsFalseWhenPropertyRelatesToNullParameter()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                null!, new Company(), instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsIgnoredReturnsTrueForIndexerProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var items = new List<int>();
            var instance = new ReadOnlyCollection<int>(items);
            var type = typeof(ReadOnlyCollection<int>);
            var propertyInfo = type.GetProperties().First(x => x.GetIndexParameters().Length > 0);

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, null!);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenIgnoreRuleMatched()
        {
            var configuration = Model.UsingDefaultConfiguration()!.AddIgnoreRule<Person>(x => x.Address!);
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, null!);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParameterMatchesReferencePropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = new Person();
            var model = new ClassMatchingNameWrapper<Person>(value);
            var propertyInfo =
                typeof(ClassMatchingNameWrapper<Person>).GetProperty(nameof(ClassMatchingNameWrapper<Person>.Value))!;
            var args = new object?[]
            {
                value
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParameterMatchesReferencePropertyWithNameMismatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var item = new Person();
            var model = new ClassMismatchingNameWrapper<Person>(item);
            var propertyInfo =
                typeof(ClassMismatchingNameWrapper<Person>).GetProperty(
                    nameof(ClassMismatchingNameWrapper<Person>.Value))!;
            var args = new object?[]
            {
                item
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParameterMatchesStringPropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = Guid.NewGuid().ToString();
            var model = new ClassMatchingNameWrapper<string>(value);
            var propertyInfo =
                typeof(ClassMatchingNameWrapper<string>).GetProperty(nameof(ClassMatchingNameWrapper<string>.Value))!;
            var args = new object?[]
            {
                value
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParameterMatchesStructPropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = new StructModel { FirstName = Guid.NewGuid().ToString() };
            var model = new StructMatchingNameWrapper<StructModel>(value);
            var propertyInfo =
                typeof(StructMatchingNameWrapper<StructModel>).GetProperty(nameof(StructMatchingNameWrapper<StructModel>
                    .Value))!;
            var args = new object?[]
            {
                value
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParameterMatchesValueTypePropertyWithNameMatch()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var value = Environment.TickCount;
            var model = new StructMatchingNameWrapper<int>(value);
            var propertyInfo =
                typeof(StructMatchingNameWrapper<int>).GetProperty(nameof(StructMatchingNameWrapper<int>.Value))!;
            var args = new object?[]
            {
                value
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, model, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParametersMatchPropertyReferenceType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.First))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenParametersMatchPropertyValueType()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = Model.Create<WithConstructorParameters>()!;
            var args = new object?[]
            {
                instance.First, instance.Id, instance.RefNumber, instance.Number, instance.Value
            };

            var propertyInfo = typeof(WithConstructorParameters).GetProperty(nameof(WithConstructorParameters.Number))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredReturnsTrueWhenPropertyValueMatchesOwningInstance()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Hashtable();
            var args = Array.Empty<object?>();

            var propertyInfo = instance.GetType().GetProperty(nameof(Hashtable.SyncRoot))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsIgnoredThrowsExceptionWhenConstructorNotFound()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var instance = new Optionals(Guid.NewGuid().ToString())
            {
                Value = Guid.NewGuid().ToString()
            };
            var propertyInfo = instance.GetType().GetProperty(nameof(Optionals.Value))!;
            var args = new object?[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.IsIgnored(configuration, instance, propertyInfo, args);

            action.Should().Throw<MissingMemberException>();
        }

        [Fact]
        public void IsIgnoredThrowsExceptionWithNullConfiguration()
        {
            var instance = new Person();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.IsIgnored(null!, instance, propertyInfo, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsIgnoredThrowsExceptionWithNullInstance()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.Address))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.IsIgnored(configuration, null!, propertyInfo, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsIgnoredThrowsExceptionWithNullPropertyInfo()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new Person();

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            Action action = () => sut.IsIgnored(configuration, instance, null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsIgnoredTreatsFailedConstructorForDefaultValueDetectionAsNull()
        {
            var configuration = Model.UsingDefaultConfiguration();
            var instance = new PropertyModel<StructConstructorException>();
            var args = Array.Empty<object?>();

            var propertyInfo = instance.GetType().GetProperty(nameof(PropertyModel<StructConstructorException>.Value))!;

            var sut = new DefaultPropertyResolver(CacheLevel.PerInstance);

            var actual = sut.IsIgnored(configuration, instance, propertyInfo, args);

            actual.Should().BeFalse();
        }

        private class ClassMatchingNameWrapper<T> where T : class
        {
            public ClassMatchingNameWrapper(T value)
            {
                Value = value;
            }

            public T Value { get; set; }
        }

        private class ClassMismatchingNameWrapper<T> where T : class
        {
            public ClassMismatchingNameWrapper(T item)
            {
                Value = item;
            }

            public T Value { get; set; }
        }

        private class PrivateProp
        {
            private Person? Person { set; get; }
        }

        private class PrivateString
        {
            public string? Name { get; }
        }

        private class PrivateValue
        {
            public Guid Id { get; }
        }

        private class ReadOnlyModelParent
        {
            public ReadOnlyModel? Child { get; set; }
        }

        private class StaticGetter
        {
            public static Person? Person { get; }
        }

        private class StaticSetter
        {
            public static Person? Person { set; get; }
        }

        private class StructMatchingNameWrapper<T> where T : struct
        {
            public StructMatchingNameWrapper(T value)
            {
                Value = value;
            }

            public T Value { get; set; }
        }

        private class StructMismatchingNameWrapper<T> where T : struct
        {
            public StructMismatchingNameWrapper(T item)
            {
                Value = item;
            }

            public T Value { get; set; }
        }
    }
}