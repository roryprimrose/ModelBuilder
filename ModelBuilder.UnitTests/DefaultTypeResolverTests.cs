namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultTypeResolverTests
    {
        private readonly OutputBuildLog _buildLog;

        public DefaultTypeResolverTests(ITestOutputHelper output)
        {
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenNoTypeMappingIsFound()
        {
            var source = typeof(string);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            configuration.Mapping<Stream, MemoryStream>();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsEmpty()
        {
            var source = typeof(string);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsNull()
        {
            var source = typeof(string);

            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsAbstractClass()
        {
            var source = typeof(Entity);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<Person>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsInterface()
        {
            var source = typeof(ITestItem);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<TestItem>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenTypeMappingFound()
        {
            var source = typeof(Stream);
            var configuration = new BuildConfiguration();

            configuration.Mapping<Stream, MemoryStream>();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<MemoryStream>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullConfiguration()
        {
            var source = typeof(string);

            var sut = new DefaultTypeResolver();

            Action action = () => sut.GetBuildType(null, source);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullSourceType()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            Action action = () => sut.GetBuildType(configuration, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}