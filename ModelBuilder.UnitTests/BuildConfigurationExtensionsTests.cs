namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddConfigurationModuleAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var config = sut.UsingModule<TestConfigurationModule>();

            config.Should().BeSameAs(sut);

            var actual = sut.PostBuildActions.OfType<DummyPostBuildAction>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void AddConfigurationModuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UsingModule<TestConfigurationModule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleThrowsExceptionWithNullConfiguration()
        {
            var module = new TestConfigurationModule();

            Action action = () => BuildConfigurationExtensions.Add(null!, module);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IConfigurationModule) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleAddsRuleToConfiguration()
        {
            var module = Substitute.For<IConfigurationModule>();

            var sut = new BuildConfiguration();

            sut.Add(module);

            module.Received().Configure(sut);
        }

        [Fact]
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategy()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var sut = Substitute.For<IBuildConfiguration>();

            sut.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = sut.Create(typeof(Guid));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null!).Create(typeof(Guid));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceType()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Create(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsInstanceCreatedByDefaultExecuteStrategy()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var sut = Substitute.For<IBuildConfiguration>();

            sut.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = sut.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null!).Create<Guid>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null!).Ignoring<Person>(x => x.Priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpression()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Ignoring<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildConfiguration()
        {
            var model = new Person();

            Action action = () => BuildConfigurationExtensions.Populate(null!, model);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Populate<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstance()
        {
            var expected = new SlimModel();

            var sut = new BuildConfiguration().UsingModule<DefaultConfigurationModule>();

            var actual = sut.Populate(expected);

            actual.Should().Be(expected);
            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void UsingExecuteStrategyReturnsExecuteStrategyWithBuildConfiguration()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            var actual = sut.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(sut);
            actual.Log.Should().BeOfType<DefaultBuildLog>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null!).UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}