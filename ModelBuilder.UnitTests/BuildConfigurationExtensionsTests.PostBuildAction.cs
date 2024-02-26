namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddPostBuildActionAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var config = sut.AddPostBuildAction<DummyPostBuildAction>();

            config.Should().Be(sut);

            var actual = sut.PostBuildActions.Single();

            actual.Should().BeOfType<DummyPostBuildAction>();
        }

        [Fact]
        public void AddPostBuildActionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddPostBuildAction<DummyPostBuildAction>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionAddsRuleToConfiguration()
        {
            var postBuildAction = new DummyPostBuildAction();

            var sut = new BuildConfiguration();

            var config = sut.Add(postBuildAction);

            config.Should().Be(sut);

            sut.PostBuildActions.Should().Contain(postBuildAction);
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullConfiguration()
        {
            var postBuildAction = new DummyPostBuildAction();

            Action action = () => BuildConfigurationExtensions.Add(null!, postBuildAction);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IPostBuildAction)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemovePostBuildActionRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.RemovePostBuildAction<DummyPostBuildAction>();

            sut.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();

            var config = sut.RemovePostBuildAction<DummyPostBuildAction>();

            config.Should().Be(sut);

            sut.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemovePostBuildAction<DummyPostBuildAction>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdatePostBuildActionThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdatePostBuildAction<DummyPostBuildAction>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdatePostBuildActionThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdatePostBuildAction<DummyPostBuildAction>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdatePostBuildActionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdatePostBuildAction<DummyPostBuildAction>(null!, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdatePostBuildActionUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyPostBuildAction
            {
                Value = expected
            };

            sut.PostBuildActions.Add(rule);

            var config = sut.UpdatePostBuildAction<DummyPostBuildAction>(x => { x.Value = expected; });

            config.Should().Be(sut);

            rule.Value.Should().Be(expected);
        }
    }
}