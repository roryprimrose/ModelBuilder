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
        public void AddPostBuildActionAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();

            var actual = sut.PostBuildActions.Single();

            actual.Should().BeOfType<DummyPostBuildAction>();
        }

        [Fact]
        public void AddPostBuildActionThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddPostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionAddsRuleToCompiler()
        {
            var postBuildAction = new DummyPostBuildAction();

            var sut = new BuildConfiguration();

            sut.Add(postBuildAction);

            sut.PostBuildActions.Should().Contain(postBuildAction);
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullCompiler()
        {
            var postBuildAction = new DummyPostBuildAction();

            Action action = () => BuildConfigurationExtensions.Add(null, postBuildAction);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IPostBuildAction) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemovePostBuildActionRemovesMultipleMatchingRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.RemovePostBuildAction<DummyPostBuildAction>();

            sut.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.RemovePostBuildAction<DummyPostBuildAction>();

            sut.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemovePostBuildAction<DummyPostBuildAction>(null);

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

            Action action = () => sut.UpdatePostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdatePostBuildActionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdatePostBuildAction<DummyPostBuildAction>(null, x =>
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

            sut.UpdatePostBuildAction<DummyPostBuildAction>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}