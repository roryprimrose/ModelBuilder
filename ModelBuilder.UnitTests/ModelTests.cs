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
        public void CreateWithReturnsCompanyWithEnumerableTypeCreatorUsageTest()
        {
            var actual = Model.Create<Company>();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeNull();
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