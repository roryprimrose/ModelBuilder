using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ModelTests
    {
        [Fact]
        public void CanAssignDefaultStrategyTest()
        {
            var strategy = Substitute.For<IBuildStrategy>();

            var existingStrategy = Model.DefaultStrategy;

            try
            {
                Model.DefaultStrategy = strategy;

                var actual = Model.DefaultStrategy;

                actual.Should().BeSameAs(strategy);
            }
            finally
            {
                Model.DefaultStrategy = existingStrategy;
            }
        }

        [Fact]
        public void CreateBuildsAndPopulatesNestedInstancesTest()
        {
            var actual = Model.Create<Person>();

            actual.Address.Should().NotBeNull();
            actual.Address.AddressLine1.Should().NotBeNullOrEmpty();
            actual.Address.AddressLine2.Should().NotBeNullOrEmpty();
            actual.Address.City.Should().NotBeNullOrEmpty();
            actual.Address.Country.Should().NotBeNullOrEmpty();
            actual.Address.State.Should().NotBeNullOrEmpty();
            actual.Address.Suburb.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void CreateReturnsGuidTest()
        {
            var actual = Model.Create<Guid>();

            actual.Should().NotBeEmpty();
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
            actual.Id.Should().NotBeEmpty();
            actual.Priority.Should().NotBe(0);
        }

        [Fact]
        public void ForReturnsDefaultExecuteStrategyTest()
        {
            var actual = Model.For<string>();

            actual.Should().BeOfType(typeof (DefaultExecuteStrategy<string>));
        }

        [Fact]
        public void IgnoringSkipsPropertyAssignmentTest()
        {
            var entity = Model.Create<Person>();
            var actual = Model.For<Person>().Ignoring(x => x.Id).Ignoring(x => x.IsActive).CreateWith(entity);

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

        [Fact]
        public void UsingReturnsSpecifiedBuildStrategyTest()
        {
            var actual = Model.Using<DummyBuildStrategy>();

            actual.Should().BeOfType(typeof (DummyBuildStrategy));
        }

        [Fact]
        public void WithReturnsSpecifiedExecuteStrategyTest()
        {
            var actual = Model.With<DummyExecuteStrategy>();

            actual.Should().BeOfType(typeof (DummyExecuteStrategy));
        }
    }
}