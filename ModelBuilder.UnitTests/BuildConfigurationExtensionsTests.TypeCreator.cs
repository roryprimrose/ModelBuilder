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
        public void AddTypeCreatorAddsTypeCreatorToConfiguration()
        {
            var sut = new BuildConfiguration();

            var config = sut.AddTypeCreator<DefaultTypeCreator>();

            config.Should().Be(sut);

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
        public void AddWithTypeCreatorAddsTypeCreatorToConfiguration()
        {
            var typeCreator = new DefaultTypeCreator();

            var sut = new BuildConfiguration();

            var config = sut.Add(typeCreator);

            config.Should().Be(sut);

            sut.TypeCreators.Should().Contain(typeCreator);
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            var typeCreator = new DefaultTypeCreator();

            Action action = () => BuildConfigurationExtensions.Add(null!, typeCreator);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullTypeCreator()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((ITypeCreator)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesMultipleMatchingTypeCreatorsFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.RemoveTypeCreator<DefaultTypeCreator>();

            sut.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesTypeCreatorsFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();

            var config = sut.RemoveTypeCreator<DefaultTypeCreator>();

            config.Should().Be(sut);

            sut.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveTypeCreator<DefaultTypeCreator>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWhenOnlyBaseClassFound()
        {
            var sut = new BuildConfiguration();
            var typeCreator = new DefaultTypeCreator();

            sut.TypeCreators.Add(typeCreator);

            Action action = () => sut.UpdateTypeCreator<ArrayTypeCreator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWhenOnlyDerivedClassFound()
        {
            var sut = new BuildConfiguration();
            var typeCreator = new ArrayTypeCreator();

            sut.TypeCreators.Add(typeCreator);

            Action action = () => sut.UpdateTypeCreator<DefaultTypeCreator>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateTypeCreatorThrowsExceptionWhenTypeCreatorNotFound()
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
        public void UpdateTypeCreatorUpdateMatchingTypeCreator()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var typeCreator = new DummyTypeCreator
            {
                Value = expected
            };

            sut.TypeCreators.Add(typeCreator);

            var config = sut.UpdateTypeCreator<DummyTypeCreator>(x => { x.Value = expected; });

            config.Should().Be(sut);

            typeCreator.Value.Should().Be(expected);
        }

        [Fact]
        public void UpdateTypeCreatorUpdateMatchingTypeCreatorMatchingExplicitType()
        {
            var maxCount = Environment.TickCount;
            var sut = new BuildConfiguration();
            var first = new DefaultTypeCreator();
            var second = new ArrayTypeCreator();

            sut.TypeCreators.Add(first);
            sut.TypeCreators.Add(second);

            var config = sut.UpdateTypeCreator<ArrayTypeCreator>(x => { x.MaxCount = maxCount; });

            config.Should().Be(sut);

            second.MaxCount.Should().Be(maxCount);
        }
    }
}