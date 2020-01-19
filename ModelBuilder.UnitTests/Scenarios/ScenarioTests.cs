﻿namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;
    using Xunit.Abstractions;
    using Location = ModelBuilder.UnitTests.Models.Location;

    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateAllNumberTypesTest()
        {
            var actual = Model.Create<Numbers>();

            actual.First.Should().NotBe(0);
            actual.Second.Should().NotBe(0);
            actual.Third.Should().NotBe(0);
            actual.Fourth.Should().NotBe(0);
            actual.Fifth.Should().NotBe(0);
            actual.Sixth.Should().NotBe(0);
            actual.Seventh.Should().NotBe(0);
            actual.Eighth.Should().NotBe(0);
            actual.Nineth.Should().NotBe(0);
            actual.Tenth.Should().NotBe(0);
        }

        [Fact]
        public void CanCreateCultureDataTest()
        {
            var actual = Model.Create<CultureData>();

            actual.Culture.Should().NotBeNull();
            actual.CultureName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanCreateCustomBuildConfigurationToCreateModelsTest()
        {
            var strategy = Model.UsingDefaultConfiguration().Set(x => x.ValueGenerators.Clear())
                .AddValueGenerator<StringValueGenerator>().AddValueGenerator<NumericValueGenerator>()
                .AddValueGenerator<BooleanValueGenerator>().AddValueGenerator<GuidValueGenerator>()
                .AddValueGenerator<DateTimeValueGenerator>().AddValueGenerator<EnumValueGenerator>();

            var actual = strategy.Create<Person>();

            Guid.Parse(actual.Address.AddressLine1).Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateEnumerableTypesTest()
        {
            var actual = Model.Create<EnumerableParent>();

            actual.Collection.Should().NotBeEmpty();
            actual.Enumerable.Should().NotBeEmpty();
            actual.InterfaceCollection.Should().NotBeEmpty();
            actual.InterfaceList.Should().NotBeEmpty();
            actual.List.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceWithoutPropertiesTest()
        {
            var actual = Model.Create<Empty>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CanGenerateArrayOfCustomTypeTest()
        {
            var actual = Model.Create<Person[]>();

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanGenerateByteArrayTest()
        {
            var actual = Model.Create<byte[]>();

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanGenerateUriDataTest()
        {
            var actual = Model.Create<Location>();

            actual.First.Should().NotBeNull();
            actual.SecondUrl.Should().NotBeNullOrWhiteSpace();
            actual.UriThird.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanPopulateCultureDataTest()
        {
            var actual = new CultureData();

            Model.Populate(actual);

            actual.Culture.Should().NotBeNull();
            actual.CultureName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateBuildsAddressUsingValidCombinationOfValuesTest()
        {
            var actual = Model.Create<Address>();

            var matchingCountries = TestData.Locations.Where(x => x.Country == actual.Country).ToList();

            matchingCountries.Should().NotBeEmpty();

            var matchingStates = matchingCountries.Where(x => x.State == actual.State).ToList();

            matchingStates.Should().NotBeEmpty();

            var matchingCities = matchingStates.Where(x => x.City == actual.City).ToList();

            matchingCities.Should().NotBeEmpty();

            var matchingPostCodes = matchingCities.Where(x => x.PostCode == actual.Postcode).ToList();

            matchingPostCodes.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateBuildsAndPopulatesNestedInstancesTest()
        {
            var actual = Model.Create<Person>();

            actual.Address.Should().NotBeNull();
            actual.Address.AddressLine1.Should().NotBeNullOrEmpty();
            actual.Address.AddressLine2.Should().NotBeNullOrEmpty();
            actual.Address.AddressLine3.Should().BeNullOrEmpty();
            actual.Address.City.Should().NotBeNullOrEmpty();
            actual.Address.Country.Should().NotBeNullOrEmpty();
            actual.Address.State.Should().NotBeNullOrEmpty();
            actual.Address.Suburb.Should().NotBeNullOrEmpty();
        }

        [Fact]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1308:NormalizeStringsToUppercase",
            Justification = "Email addresses are lower case by convention.")]
        public void CreateBuildsEmailUsingValidCombinationOfValuesTest()
        {
            var actual = Model.Create<EmailParts>();

            var expected = actual.FirstName + "." + actual.LastName + "@" + actual.Domain;

            expected = expected.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

            actual.Email.Should().Be(expected);
        }

        [Fact]
        public void CreateBuildsLogOfConstructionActionsTest()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildsLogOfConstructionActionsWhereModelConstructorsAreUsedTest()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<WithValueParameters>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildLogOfIgnoreRuleTest()
        {
            var strategy = Model.Ignoring<Company>(x => x.Address)
                .UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().Contain("Ignoring");

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildLogOfPostActionsTest()
        {
            var strategy = Model.UsingDefaultConfiguration().UsingModule<TestConfigurationModule>().UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().Contain(typeof(DummyPostBuildAction).FullName);

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateDoesNotPopulateReadOnlyValueTypePropertiesTest()
        {
            var actual = Model.Create<ReadOnlyParent>();

            actual.PrivateValue.Should().Be(0);
        }

        [Fact]
        public void CreateDoesNotPopulateStaticPropertiesTest()
        {
            var actual = Model.Create<WithStatic>();

            actual.First.Should().NotBeNullOrWhiteSpace();
            WithStatic.Second.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateDoesNotSetPropertiesProvidedByConstructorTest()
        {
            var args = new object[]
            {
                new Company(), Guid.NewGuid(), 123, 456, true
            };

            var actual = Model.Create<WithConstructorParameters>(args);

            actual.First.Should().BeSameAs(args[0]);
            actual.Id.Should().Be((Guid) args[1]);
            actual.RefNumber.Should().Be((int?) args[2]);
            actual.Number.Should().Be((int) args[3]);
            actual.Value.Should().Be((bool) args[4]);
        }

        [Fact]
        public void CreatePopulatesBaseClassPropertiesTest()
        {
            var actual = Model.Create<SpecificCompany>();

            actual.Email.Should().NotBeNullOrWhiteSpace();
            actual.Address.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatePopulatesReadOnlyReferenceTypePropertiesTest()
        {
            var actual = Model.Create<ReadOnlyParent>();

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.ReadOnlyPerson.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void CreateReadOnlyCollectionWithAutoPopulatedItemsTest()
        {
            var actual = Model.Create<ReadOnlyCollection<int>>();

            actual.Should().HaveCount(EnumerableTypeCreator.DefaultAutoPopulateCount);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCollectionWithAutoPopulatedItemsInstanceTest()
        {
            var actual = Model.Create<ICollection<int>>();

            actual.Should().HaveCount(EnumerableTypeCreator.DefaultAutoPopulateCount);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCompanyWithEnumerableTypeCreatorUsageTest()
        {
            var actual = Model.Create<Company>();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsEnumerableWithAutoPopulatedItemsInstanceTest()
        {
            var actual = Model.Create<IEnumerable<int>>().ToList();

            actual.Should().HaveCount(EnumerableTypeCreator.DefaultAutoPopulateCount);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsGuidTest()
        {
            var actual = Model.Create<Guid>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsListWithAutoPopulatedItemsInstanceTest()
        {
            var actual = Model.Create<IList<int>>();

            actual.Should().HaveCount(EnumerableTypeCreator.DefaultAutoPopulateCount);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsPersonCreatedWithArgumentsTest()
        {
            var entity = Model.Create<Person>();
            var actual = Model.Create<Person>(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.PersonalEmail.Should().Match("*@*.*");
            actual.WorkEmail.Should().Match("*@*.*");
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void CreateReturnsSimpleInstanceTest()
        {
            var actual = Model.Create<Simple>();

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Id.Should().NotBeEmpty();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void CreateReturnsStringTest()
        {
            var actual = Model.Create<string>();

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatesCircularReferenceWithInstanceFromBuildChainTest()
        {
            var actual = Model.Create<Top>();

            actual.Should().NotBeNull();
            actual.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.Should().NotBeNull();
            actual.Next.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Should().NotBeNull();
            actual.Next.End.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Root.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreatesDirectCircularReferenceWithInstanceFromBuildChainTest()
        {
            var actual = Model.Create<SelfReferrer>();

            actual.Should().NotBeNull();
            actual.Id.Should().NotBeEmpty();
            actual.Self.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreateSetsPropertyValuesWhenConstructorParametersHaveDefaultValuesTest()
        {
            var configuration = Model.UsingDefaultConfiguration();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration);

            var args = new object[]
            {
                null, Guid.Empty, null, 0, false
            };

            var actual = Model.Create<WithConstructorParameters>(args);

            actual.First.Should().NotBeNull();
            actual.Id.Should().NotBeEmpty();
            (actual.RefNumber == null || actual.RefNumber != 0).Should().BeTrue();
            actual.Number.Should().NotBe(0);
        }

        [Fact]
        public void CreateSetsPropertyValueWhenNoMatchOnConstructorParameterTest()
        {
            var configuration = Model.UsingDefaultConfiguration();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration);

            var args = new object[]
            {
                Guid.NewGuid().ToString()
            };

            var actual = Model.Create<WithMixedValueParameters>(args);

            actual.FirstName.Should().NotBe((string) args[0]);
            actual.LastName.Should().NotBe((string) args[0]);
        }

        [Fact]
        public void CreatesPropertyOfSameTypeWithCreatedInstanceTest()
        {
            var actual = Model.Create<Looper>();

            actual.Should().NotBeNull();
            actual.Stuff.Should().NotBeNullOrWhiteSpace();
            actual.Other.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreateThrowsExceptionWhenPropertyCannotBeCreatedTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.CanCreate(typeof(Address), "Address", Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(typeof(Address), "Address", Arg.Any<IExecuteStrategy>())
                .Throws(new InvalidOperationException());

            var configuration = Model.UsingDefaultConfiguration().Add(typeCreator);

            var target = new DefaultExecuteStrategy<Company>();

            target.Initialize(configuration);

            Action action = () => target.Create();

            var exception = action.Should().Throw<BuildException>().Where(x => x.Message != null)
                .Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreatingRuleAppliesToAllItemsCreatedTest()
        {
            var expected = Guid.NewGuid();

            var strategy = Model.UsingDefaultConfiguration().AddCreationRule<Person>(x => x.Id, 100, expected)
                ;

            var actual = strategy.Create<List<Person>>();

            actual.All(x => x.Id == expected).Should().BeTrue();
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentOfNestedObjectsTest()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).Ignoring<Address>(x => x.AddressLine1)
                .Create<Person>();

            actual.Should().NotBeNull();
            actual.FirstName.Should().BeNull();
            actual.Address.AddressLine1.Should().BeNull();
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentTest()
        {
            var entity = Model.Create<Person>();
            var actual = Model.Ignoring<Person>(x => x.Id).Ignoring<Person>(x => x.IsActive).Create<Person>(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
            actual.Id.Should().Be(entity.Id);
            actual.IsActive.Should().Be(entity.IsActive);
        }

        [Fact]
        public void MailinatorEmailGeneratorIsAssignedAgainstAllInstancesTest()
        {
            var configuration = Model.UsingDefaultConfiguration().RemoveValueGenerator<EmailValueGenerator>()
                .AddValueGenerator<MailinatorEmailValueGenerator>();

            var actual = configuration.Create<List<Person>>();

            actual.All(x => x.PersonalEmail.EndsWith("@mailinator.com", StringComparison.OrdinalIgnoreCase)).Should()
                .BeTrue();
            actual.All(x => x.WorkEmail.EndsWith("@mailinator.com", StringComparison.OrdinalIgnoreCase)).Should()
                .BeTrue();
        }

        [Fact]
        public void PopulateCanBuildExistingInstanceTest()
        {
            var entity = new Person();
            var actual = Model.Populate(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Id.Should().NotBeEmpty();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void PopulateHandlesDirectCircularReferenceWithInstanceFromBuildChainTest()
        {
            var expected = new SelfReferrer();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Id.Should().NotBeEmpty();
            actual.Self.Should().BeSameAs(actual);
        }

        [Fact]
        public void PopulateIgnoresEmptyInstanceTest()
        {
            var expected = new Empty();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateOnlySetsPublicInstancePropertiesOnlyTest()
        {
            var expected = new PropertyScope();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Public.Should().NotBeEmpty();
            actual.PrivateSet.Should().BeEmpty();
            actual.CannotSetValue.Should().BeEmpty();
            PropertyScope.GlobalValue.Should().BeEmpty();
        }

        [Fact]
        public void PopulatePopulatesReadOnlyReferencePropertiesTest()
        {
            var actual = new ReadOnlyParent();

            actual = Model.Populate(actual);

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void PopulateUsesResolvedTypeCreatorToPopulateInstanceTest()
        {
            var actual = new List<Person>();

            actual = Model.Populate(actual);

            actual.Should().NotBeEmpty();
            actual.Count.Should().Be(EnumerableTypeCreator.DefaultAutoPopulateCount);
        }

        [Fact]
        public void UsesBuildLogInstancePerExecutionPipelineTest()
        {
            var configuration = Model.UsingDefaultConfiguration();

            const int MaxTasks = 100;
            var tasks = new List<Task<string>>(MaxTasks);

            for (var index = 0; index < MaxTasks; index++)
            {
                var loopIndex = index;
                var task = Task<string>.Factory.StartNew(
                    () =>
                    {
                        var strategy = configuration.UsingExecuteStrategy<DefaultExecuteStrategy<Empty>>();

                        strategy.Create();

                        return "Iteration " + loopIndex + " on thread " + Thread.CurrentThread.ManagedThreadId +
                               Environment.NewLine + strategy.Log.Output;
                    });

                tasks.Add(task);
            }

            Task.WhenAll(tasks).Wait();

            for (var index = 0; index < MaxTasks; index++)
            {
                _output.WriteLine(tasks[index].Result + Environment.NewLine);
            }
        }

        private class Bottom
        {
            public Top Root { get; set; }

            public string Value { get; set; }
        }

        private class Child
        {
            public Bottom End { get; set; }

            public string Value { get; set; }
        }

        private class Looper
        {
            public Looper Other { get; set; }

            public string Stuff { get; set; }
        }

        private class Top
        {
            public Child Next { get; set; }

            public string Value { get; set; }
        }
    }
}