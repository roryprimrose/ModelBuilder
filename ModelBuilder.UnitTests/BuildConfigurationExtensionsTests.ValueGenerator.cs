namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddValueGeneratorAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();

            var actual = sut.ValueGenerators.Single();

            actual.Should().BeOfType<StringValueGenerator>();
        }

        [Fact]
        public void AddValueGeneratorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddValueGenerator<StringValueGenerator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorAddsRuleToConfiguration()
        {
            var rule = new StringValueGenerator();

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.ValueGenerators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullConfiguration()
        {
            var rule = new StringValueGenerator();

            Action action = () => BuildConfigurationExtensions.Add(null!, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IValueGenerator) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();
            sut.AddValueGenerator<StringValueGenerator>();
            sut.AddValueGenerator<StringValueGenerator>();
            sut.RemoveValueGenerator<StringValueGenerator>();

            sut.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();
            sut.RemoveValueGenerator<StringValueGenerator>();

            sut.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveValueGenerator<StringValueGenerator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateValueGeneratorThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdateValueGenerator<DummyValueGenerator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateValueGeneratorThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdateValueGenerator<DummyValueGenerator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateValueGeneratorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateValueGenerator<DummyValueGenerator>(null!, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateValueGeneratorUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyValueGenerator
            {
                Value = expected
            };

            sut.ValueGenerators.Add(rule);

            sut.UpdateValueGenerator<DummyValueGenerator>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}