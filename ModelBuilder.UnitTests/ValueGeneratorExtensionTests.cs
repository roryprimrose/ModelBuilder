using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ValueGeneratorExtensionTests
    {
        [Fact]
        public void GenerateReturnsGeneratorValueWithoutContextInformationTest()
        {
            var type = typeof (Guid);
            var model = new Person();

            var target = Substitute.For<IValueGenerator>();

            target.Generate(type, null, null).Returns(model);

            var actual = target.Generate(type);

            actual.Should().Be(model);
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullGeneratorTest()
        {
            var type = typeof (Guid);

            var target = (IValueGenerator) null;

            Action action = () => target.Generate(type);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedReturnsGeneratorValueWithoutContextInformationTest()
        {
            var type = typeof (Guid);

            var target = Substitute.For<IValueGenerator>();

            target.IsSupported(type, null, null).Returns(true);

            var actual = target.IsSupported(type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullGeneratorTest()
        {
            var type = typeof (Guid);

            var target = (IValueGenerator) null;

            Action action = () => target.IsSupported(type);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}