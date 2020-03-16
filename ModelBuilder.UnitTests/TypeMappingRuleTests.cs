namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class TypeMappingRuleTests
    {
        [Fact]
        public void ReturnsConstructorParametersAsProperties()
        {
            var sourceType = typeof(Stream);
            var targetType = typeof(MemoryStream);

            var sut = new TypeMappingRule(sourceType, targetType);

            sut.SourceType.Should().Be(sourceType);
            sut.TargetType.Should().Be(targetType);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullSourceType()
        {
            var targetType = typeof(MemoryStream);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypeMappingRule(null, targetType);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTargetType()
        {
            var sourceType = typeof(Stream);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypeMappingRule(sourceType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenOrderOfTargetTypeAndSourceTypeAreBackward()
        {
            var sourceType = typeof(MemoryStream);
            var targetType = typeof(Stream);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypeMappingRule(sourceType, targetType);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenTargetTypeNotAssignableToSourceType()
        {
            var sourceType = typeof(Stream);
            var targetType = typeof(Person);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypeMappingRule(sourceType, targetType);

            action.Should().Throw<ArgumentException>();
        }
    }
}