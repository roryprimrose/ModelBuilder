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
            var generator = new StringValueGenerator();

            var sut = new BuildConfiguration();

            sut.Add(generator);

            sut.ValueGenerators.Should().Contain(generator);
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullConfiguration()
        {
            var generator = new StringValueGenerator();

            Action action = () => BuildConfigurationExtensions.Add(null!, generator);

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
        public void UpdateValueGeneratorThrowsExceptionWhenOnlyBaseClassFound()
        {
            var sut = new BuildConfiguration();
            var generator = new NumericValueGenerator();

            sut.ValueGenerators.Add(generator);

            Action action = () => sut.UpdateValueGenerator<CountValueGenerator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateValueGeneratorThrowsExceptionWhenOnlyDerivedClassFound()
        {
            var sut = new BuildConfiguration();
            var generator = new CountValueGenerator();

            sut.ValueGenerators.Add(generator);

            Action action = () => sut.UpdateValueGenerator<NumericValueGenerator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateValueGeneratorThrowsExceptionWhenGeneratorNotFound()
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
            var generator = new DummyValueGenerator
            {
                Value = expected
            };

            sut.ValueGenerators.Add(generator);

            sut.UpdateValueGenerator<DummyValueGenerator>(x => { x.Value = expected; });

            generator.Value.Should().Be(expected);
        }

        [Fact]
        public void UpdateValueGeneratorUpdateMatchingRuleMatchingExplicitType()
        {
            var sut = new BuildConfiguration();
            var first = new CountValueGenerator();
            var second = new NumericValueGenerator();

            sut.ValueGenerators.Add(first);
            sut.ValueGenerators.Add(second);

            sut.UpdateValueGenerator<NumericValueGenerator>(x => { x.AllowNull = true; });

            first.AllowNull.Should().BeFalse();
            second.AllowNull.Should().BeTrue();
        }
    }
}