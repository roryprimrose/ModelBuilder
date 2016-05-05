using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Xunit.Abstractions;

namespace ModelBuilder.UnitTests
{
    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanGenerateByteArrayTest()
        {
            var actual = Model.Create<byte[]>();
            
            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanGenerateArrayOfCustomTypeTest()
        {
            var actual = Model.Create<Person[]>();

            actual.Length.Should().BeGreaterThan(0);
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
        public void CanCreateCustomBuildStrategyToCreateModelsTest()
        {
            var strategy =
                Model.DefaultBuildStrategy.Clone()
                    .Set(x => x.ValueGenerators.Clear())
                    .AddValueGenerator<StringValueGenerator>()
                    .AddValueGenerator<NumericValueGenerator>()
                    .AddValueGenerator<BooleanValueGenerator>()
                    .AddValueGenerator<GuidValueGenerator>()
                    .AddValueGenerator<DateTimeValueGenerator>()
                    .AddValueGenerator<EnumValueGenerator>()
                    .Compile();

            var actual = strategy.Create<Person>();

            Guid.Parse(actual.Address.AddressLine1).Should().NotBeEmpty();
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
        public void CreateBuildsLogOfConstructionActionsTest()
        {
            var strategy = Model.For<Company>();

            strategy.Create();

            var actual = strategy.BuildStrategy.BuildLog.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
        }

        [Fact]
        public void CreateBuildsLogOfConstructionActionsWhereModelConstructorsAreUsedTest()
        {
            var strategy = Model.For<WithValueParameters>();

            strategy.Create();

            var actual = strategy.BuildStrategy.BuildLog.Output;

            actual.Should().NotBeNullOrWhiteSpace();

            _output.WriteLine(actual);
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
        public void CreateReturnsEnumerableWithAutoPopulatedItemsInstanceTest()
        {
            var actual = Model.Create<IEnumerable<int>>();

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
        public void CreateReturnsSimpleInstanceTest()
        {
            var actual = Model.Create<Simple>();

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default(DateTime));
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
        public void CreateWithReturnsCompanyWithEnumerableTypeCreatorUsageTest()
        {
            var actual = Model.Create<Company>();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateWithReturnsPersonCreatedWithArgumentsTest()
        {
            var entity = Model.Create<Person>();
            var actual = Model.CreateWith<Person>(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default(DateTime));
            actual.PersonalEmail.Should().Match("*@*.*");
            actual.WorkEmail.Should().Match("*@*.*");
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void CreateThrowsExceptionWhenPropertyCannotBeCreatedTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.IsSupported(typeof(Address), "Address", Arg.Any<object>()).Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(typeof(Address), "Address", Arg.Any<object>()).Throws(new InvalidOperationException());

            var buildStrategy = new DefaultBuildStrategy().Clone().Add(typeCreator).Compile();

            var target = new DefaultExecuteStrategy<Company> { BuildStrategy = buildStrategy };

            Action action = () => target.CreateWith();

            var exception = action.ShouldThrow<BuildException>().Where(x => x.Message != null).Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void ForReturnsDefaultExecuteStrategyTest()
        {
            var actual = Model.For<string>();

            actual.Should().BeOfType(typeof(DefaultExecuteStrategy<string>));
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentOfNestedObjectsTest()
        {
            var actual =
                Model.Ignoring<Person>(x => x.FirstName).Ignoring<Address>(x => x.AddressLine1).Create<Person>();

            actual.Should().NotBeNull();
            actual.FirstName.Should().BeNull();
            actual.Address.AddressLine1.Should().BeNull();
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentTest()
        {
            var entity = Model.Create<Person>();
            var actual = Model.Ignoring<Person>(x => x.Id).Ignoring<Person>(x => x.IsActive).CreateWith<Person>(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default(DateTime));
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Priority.Should().NotBe(0);
            actual.Id.Should().Be(entity.Id);
            actual.IsActive.Should().Be(entity.IsActive);
        }

        [Fact]
        public void PopulateCanBuildExistingInstanceTest()
        {
            var entity = new Person();
            var actual = Model.Populate(entity);

            actual.Should().NotBeNull();
            actual.DOB.Should().NotBe(default(DateTime));
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
            actual.LastName.Should().NotBeNullOrWhiteSpace();
            actual.Id.Should().NotBeEmpty();
            actual.Priority.Should().NotBe(0);
        }
    }
}