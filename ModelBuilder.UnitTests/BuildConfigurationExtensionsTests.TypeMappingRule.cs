namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddTypeMappingRuleAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();

            var actual = sut.TypeMappingRules.Single();

            actual.Should().BeOfType<DummyTypeMappingRule>();
        }

        [Fact]
        public void AddTypeMappingRuleCreatesRuleWithGenericTypes()
        {
            var configuration = new BuildConfiguration();

            var actual = configuration.AddTypeMappingRule<Stream, MemoryStream>();

            actual.Should().Be(configuration);
            actual.TypeMappingRules.Should().HaveCount(1);

            var item = actual.TypeMappingRules.First();

            item.SourceType.Should().Be<Stream>();
            item.TargetType.Should().Be<MemoryStream>();
        }

        [Fact]
        public void AddTypeMappingRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddTypeMappingRule<DummyTypeMappingRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTypeMappingRuleWithGenericTypesThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddTypeMappingRule<Stream, MemoryStream>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleAddsRuleToConfiguration()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.TypeMappingRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullConfiguration()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            Action action = () => BuildConfigurationExtensions.Add(null!, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((TypeMappingRule) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsNewBuildConfigurationWithTypeMappingRuleAppended()
        {
            var typeMappings = new Collection<TypeMappingRule>();

            var sut = Substitute.For<IBuildConfiguration>();

            sut.TypeMappingRules.Returns(typeMappings);

            var actual = sut.Mapping<Stream, MemoryStream>();

            actual.TypeMappingRules.Should().NotBeEmpty();

            var matchingRule = actual.TypeMappingRules.FirstOrDefault(
                x => x.SourceType == typeof(Stream) && x.TargetType == typeof(MemoryStream));

            matchingRule.Should().NotBeNull();
        }

        [Fact]
        public void MappingThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null!).Mapping<Stream, MemoryStream>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.RemoveTypeMappingRule<DummyTypeMappingRule>();

            sut.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.RemoveTypeMappingRule<DummyTypeMappingRule>();

            sut.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveTypeMappingRule<DummyTypeMappingRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}