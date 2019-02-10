namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xunit;

    public class TypeMappingRuleTests
    {
        [Fact]
        public void ReturnsConstructorParametersAsPropertiesTest()
        {
            var sourceType = typeof(Stream);
            var targetType = typeof(MemoryStream);

            var sut = new TypeMappingRule(sourceType, targetType);

            sut.SourceType.Should().Be(sourceType);
            sut.TargetType.Should().Be(targetType);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullSourceTypeTest()
        {
            var targetType = typeof(MemoryStream);

            Action action = () => new TypeMappingRule(null, targetType);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTargetTypeTest()
        {
            var sourceType = typeof(Stream);

            Action action = () => new TypeMappingRule(sourceType, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}