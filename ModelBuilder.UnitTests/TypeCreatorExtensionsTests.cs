using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class TypeCreatorExtensionTests
    {
        [Fact]
        public void CreateReturnsGeneratorValueWithoutContextInformationTest()
        {
            var type = typeof (Guid);
            var model = new Person();

            var target = Substitute.For<ITypeCreator>();

            target.Create(type, null, null).Returns(model);

            var actual = target.Create(type);

            actual.Should().Be(model);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullGeneratorTest()
        {
            var type = typeof (Guid);

            var target = (ITypeCreator) null;

            Action action = () => target.Create(type);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedReturnsGeneratorValueWithoutContextInformationTest()
        {
            var type = typeof (Guid);

            var target = Substitute.For<ITypeCreator>();

            target.IsSupported(type, null, null).Returns(true);

            var actual = target.IsSupported(type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullGeneratorTest()
        {
            var type = typeof (Guid);

            var target = (ITypeCreator) null;

            Action action = () => target.IsSupported(type);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}