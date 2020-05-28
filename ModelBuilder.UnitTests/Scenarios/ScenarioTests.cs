namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.UnitTests.ValueGenerators;
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
        public void CanCreateCultureData()
        {
            var actual = Model.Create<CultureData>()!;

            actual.Culture.Should().NotBeNull();
            actual.CultureName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanCreateCustomBuildConfigurationToCreateModels()
        {
            var strategy = Model.UsingDefaultConfiguration().Set(x => x.ValueGenerators.Clear())
                .AddValueGenerator<StringValueGenerator>().AddValueGenerator<NumericValueGenerator>()
                .AddValueGenerator<BooleanValueGenerator>().AddValueGenerator<GuidValueGenerator>()
                .AddValueGenerator<DateTimeValueGenerator>().AddValueGenerator<EnumValueGenerator>().UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            var actual = strategy.WriteLog(_output.WriteLine).Create()!;

            Guid.Parse(actual.Address!.AddressLine1!).Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateEnumValue()
        {
            var actual = Model.Create<SimpleEnum>();

            Enum.IsDefined(typeof(SimpleEnum), actual).Should().BeTrue();
        }

        [Fact]
        public void CanCreateGenericType()
        {
            var actual = Model.Create<GenericContainer<Office>>()!;

            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Email.Should().NotBeNullOrWhiteSpace();
            actual.Value.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateGuidValue()
        {
            var actual = Model.Create<Guid>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstance()
        {
            var actual = Model.Create<Address>()!;

            actual.AddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.City.Should().NotBeNullOrWhiteSpace();
            actual.Country.Should().NotBeNullOrWhiteSpace();
            actual.Postcode.Should().NotBeNullOrWhiteSpace();
            actual.State.Should().NotBeNullOrWhiteSpace();
            actual.Suburb.Should().NotBeNullOrWhiteSpace();
            actual.TimeZone.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanCreateInstanceWithoutProperties()
        {
            var actual = Model.Create<Empty>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateInstanceWithParameters()
        {
            var actual = Model.Create<ReadOnlyModel>()!;

            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateWithMultipleNames()
        {
            var actual = Model.Create<Names>()!;

            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.MiddleName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanGenerateUriData()
        {
            var actual = Model.Create<Location>()!;

            actual.First.Should().NotBeNull();
            actual.SecondUrl.Should().NotBeNullOrWhiteSpace();
            actual.UriThird.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanPopulateCultureData()
        {
            var actual = new CultureData();

            Model.Populate(actual);

            actual.Culture.Should().NotBeNull();
            actual.CultureName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanPopulateExistingInstance()
        {
            var expected = new SlimModel();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateBuildLogOfIgnoreRule()
        {
            var strategy = Model.Ignoring<Company>(x => x.Address!)
                .UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().Contain("Ignoring");

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildLogOfPostActions()
        {
            var strategy = Model.UsingDefaultConfiguration().UsingModule<TestConfigurationModule>()
                .UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().Contain(typeof(DummyPostBuildAction).FullName);

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildsAddressUsingValidCombinationOfValues()
        {
            var actual = Model.Create<Address>()!;

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
        public void CreateBuildsAndPopulatesNestedInstances()
        {
            var actual = Model.Create<Person>()!;

            actual.Address.Should().NotBeNull();
            actual.Address!.AddressLine1.Should().NotBeNullOrEmpty();
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
        public void CreateBuildsEmailUsingValidCombinationOfValues()
        {
            var actual = Model.Create<EmailParts>()!;

            var firstName = EmailValueGenerator.SpecialCharacters.Replace(actual.FirstName, string.Empty);
            var lastName = EmailValueGenerator.SpecialCharacters.Replace(actual.LastName, string.Empty);

            var expected = (firstName + "." + lastName + "@" + actual.Domain).ToLowerInvariant();

            actual.Email.Should().Be(expected);
        }

        [Fact]
        public void CreateBuildsLogOfConstructionActions()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Company>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildsLogOfConstructionActionsWhereModelConstructorsAreUsed()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<WithValueParameters>>();

            strategy.Create();

            var actual = strategy.Log.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateDoesNotPopulateReadOnlyValueTypeProperties()
        {
            var actual = Model.Create<ReadOnlyParent>()!;

            actual.PrivateValue.Should().Be(0);
        }

        [Fact]
        public void CreateDoesNotPopulateStaticProperties()
        {
            var actual = Model.Create<WithStatic>()!;

            actual.First.Should().NotBeNullOrWhiteSpace();
            WithStatic.Second.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateDoesNotSetPropertiesProvidedByConstructor()
        {
            var args = new object[]
            {
                new Company(), Guid.NewGuid(), 123, 456, true
            };

            var actual = Model.Create<WithConstructorParameters>(args)!;

            actual.First.Should().BeSameAs(args[0]);
            actual.Id.Should().Be((Guid) args[1]);
            actual.RefNumber.Should().Be((int?) args[2]);
            actual.Number.Should().Be((int) args[3]);
            actual.Value.Should().Be((bool) args[4]);
        }

        [Fact]
        public void CreatePopulatesBaseClassProperties()
        {
            var actual = Model.Create<SpecificCompany>()!;

            actual.Email.Should().NotBeNullOrWhiteSpace();
            actual.Address.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatePopulatesReadOnlyReferenceTypeProperties()
        {
            var actual = Model.Create<ReadOnlyParent>()!;

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.ReadOnlyPerson.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsGuid()
        {
            var actual = Model.Create<Guid>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsPersonCreatedWithArguments()
        {
            var entity = Model.Create<Person>()!;
            var actual = Model.Create<Person>(entity)!;

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.PersonalEmail.Should().Match("*@*.*");
            actual.WorkEmail.Should().Match("*@*.*");
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void CreateReturnsSimpleInstance()
        {
            var actual = Model.Create<Simple>()!;

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Id.Should().NotBeEmpty();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void CreateReturnsString()
        {
            var actual = Model.Create<string>();

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatesAgeFromDob()
        {
            var actual = Model.Create<AgeFromDob>()!;

            var span = DateTime.Now.Subtract(actual.DateOfBirth);
            var years = Convert.ToInt32(Math.Floor(span.TotalDays / 365));

            actual.Age.Should().Be(years);
        }

        [Fact]
        public void CreatesCircularReferenceWithInstanceFromBuildChain()
        {
            var actual = Model.Create<Top>()!;

            actual.Should().NotBeNull();
            actual.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.Should().NotBeNull();
            actual.Next!.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Should().NotBeNull();
            actual.Next.End!.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Root.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreatesDirectCircularReferenceWithInstanceFromBuildChain()
        {
            var actual = Model.Create<SelfReferrer>()!;

            actual.Should().NotBeNull();
            actual.Id.Should().NotBeEmpty();
            actual.Self.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreateSetsPropertyValuesWhenConstructorParametersHaveDefaultValues()
        {
            var configuration = Model.UsingDefaultConfiguration();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(configuration);

            var args = new object[]
            {
                null!, Guid.Empty, null!, 0, false
            };

            var actual = Model.Create<WithConstructorParameters>(args)!;

            actual.First.Should().NotBeNull();
            actual.Id.Should().NotBeEmpty();
            (actual.RefNumber == null || actual.RefNumber != 0).Should().BeTrue();
            actual.Number.Should().NotBe(0);
        }

        [Fact]
        public void CreateSetsPropertyValueWhenNoMatchOnConstructorParameter()
        {
            var configuration = Model.UsingDefaultConfiguration();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(configuration);

            var args = new object[]
            {
                Guid.NewGuid().ToString()
            };

            var actual = Model.Create<WithMixedValueParameters>(args)!;

            actual.FirstName.Should().NotBe((string) args[0]);
            actual.LastName.Should().NotBe((string) args[0]);
        }

        [Fact]
        public void CreatesPropertyOfSameTypeWithCreatedInstance()
        {
            var actual = Model.Create<Looper>()!;

            actual.Should().NotBeNull();
            actual.Stuff.Should().NotBeNullOrWhiteSpace();
            actual.Other.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreatesUsesConfigurationToPopulateParameter()
        {
            var value = Guid.NewGuid().ToString();

            var actual = Model.UsingDefaultConfiguration()
                .AddCreationRule(typeof(string), "firstName", value, int.MaxValue)
                .Create<OrderedConstructorParameters>()!;

            actual.FirstName.Should().Be(value);
        }

        [Fact]
        public void CreatesUsesConfigurationToPopulateProperty()
        {
            var value = Guid.NewGuid().ToString();

            var actual = Model.UsingDefaultConfiguration()
                .AddCreationRule(typeof(string), "FirstName", value, int.MaxValue)
                .Create<Person>()!;

            actual.FirstName.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWhenPropertyCannotBeCreated()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.CanCreate(Arg.Any<IBuildConfiguration>(), Arg.Any<IBuildChain>(),
                    Arg.Is<PropertyInfo>(x => x.DeclaringType == typeof(Office) && x.Name == nameof(Office.Address)))
                .Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(Arg.Any<IExecuteStrategy>(),
                    Arg.Is<PropertyInfo>(x => x.DeclaringType == typeof(Office) && x.Name == nameof(Office.Address)),
                    null!)
                .Throws(new InvalidOperationException());

            var configuration = Model.UsingDefaultConfiguration().Add(typeCreator);

            var sut = new DefaultExecuteStrategy<Office>();

            sut.Initialize(configuration);

            Action action = () => sut.Create();

            var exception = action.Should().Throw<BuildException>().Where(x => x.Message != null!)
                .Where(x => x.BuildLog != null!).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreatingRuleAppliesToAllItemsCreated()
        {
            var expected = Guid.NewGuid();

            var strategy = Model.UsingDefaultConfiguration().AddCreationRule<Person>(x => x.Id, expected, 100);

            var actual = strategy.Create<List<Person>>();

            actual.All(x => x.Id == expected).Should().BeTrue();
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignment()
        {
            var entity = Model.Create<Person>()!;
            var actual = Model.Ignoring<Person>(x => x.Id).Ignoring<Person>(x => x.IsActive).Create<Person>(entity)!;

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default);
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
            actual.Id.Should().Be(entity.Id);
            actual.IsActive.Should().Be(entity.IsActive);
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentOfNestedObjects()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName!).Ignoring<Address>(x => x.AddressLine1!)
                .Create<Person>()!;

            actual.Should().NotBeNull();
            actual.FirstName.Should().BeNull();
            actual.Address!.AddressLine1.Should().BeNull();
        }

        [Fact]
        public void MailinatorEmailGeneratorIsAssignedAgainstAllInstances()
        {
            var configuration = Model.UsingDefaultConfiguration().RemoveValueGenerator<EmailValueGenerator>()
                .AddValueGenerator<MailinatorEmailValueGenerator>();

            var actual = configuration.Create<List<Person>>();

            actual.All(x => x.PersonalEmail!.EndsWith("@mailinator.com", StringComparison.OrdinalIgnoreCase)).Should()
                .BeTrue();
            actual.All(x => x.WorkEmail!.EndsWith("@mailinator.com", StringComparison.OrdinalIgnoreCase)).Should()
                .BeTrue();
        }

        [Fact]
        public void PopulateCanBuildExistingInstance()
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
        public void PopulateHandlesDirectCircularReferenceWithInstanceFromBuildChain()
        {
            var expected = new SelfReferrer();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Id.Should().NotBeEmpty();
            actual.Self.Should().BeSameAs(actual);
        }

        [Fact]
        public void PopulateIgnoresEmptyInstance()
        {
            var expected = new Empty();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateOnlySetsPublicInstancePropertiesOnly()
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
        public void PopulatePopulatesReadOnlyReferenceProperties()
        {
            var actual = new ReadOnlyParent();

            actual = Model.Ignoring<ReadOnlyParent>(x => x.RestrictedPeople).Populate(actual);

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void PopulateUsesResolvedTypeCreatorToPopulateInstance()
        {
            var actual = new List<Person>();

            actual = Model.Populate(actual);

            actual.Should().NotBeEmpty();
            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
        }

        [Fact]
        public void UsesBuildLogInstancePerExecutionPipeline()
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
            public Top? Root { get; set; }

            public string? Value { get; set; }
        }

        private class Child
        {
            public Bottom? End { get; set; }

            public string? Value { get; set; }
        }

        private class Looper
        {
            public Looper? Other { get; set; }

            public string? Stuff { get; set; }
        }

        private class Top
        {
            public Child? Next { get; set; }

            public string? Value { get; set; }
        }
    }
}