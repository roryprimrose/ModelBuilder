namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddTypeCreatorAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();

            var actual = sut.TypeCreators.Single();

            actual.Should().BeOfType<DefaultTypeCreator>();
        }

        [Fact]
        public void AddTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddTypeCreator<DefaultTypeCreator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorAddsRuleToConfiguration()
        {
            var rule = new DefaultTypeCreator();

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.TypeCreators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            var rule = new DefaultTypeCreator();

            Action action = () => BuildConfigurationExtensions.Add(null!, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((ITypeCreator) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.RemoveTypeCreator<DefaultTypeCreator>();

            sut.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.RemoveTypeCreator<DefaultTypeCreator>();

            sut.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveTypeCreator<DefaultTypeCreator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdateTypeCreator<DummyTypeCreator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdateTypeCreator<DummyTypeCreator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateTypeCreator<DummyTypeCreator>(null!, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateTypeCreatorUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyTypeCreator
            {
                Value = expected
            };

            sut.TypeCreators.Add(rule);

            sut.UpdateTypeCreator<DummyTypeCreator>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}