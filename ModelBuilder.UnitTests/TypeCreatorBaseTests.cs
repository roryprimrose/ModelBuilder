using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class TypeCreatorBaseTests
    {
        [Fact]
        public void IsSupportedReturnsFalseForAbstractTypeTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.IsSupported(typeof (BuildStrategyBase), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsFalseForInterfaceTypeTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.IsSupported(typeof (IBuildStrategy), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsFalseForValueTypeTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.IsSupported(typeof (int), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueForReferenceTypeTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.IsSupported(typeof (MemoryStream), null, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultTypeCreator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateReturnsProvidedInstanceTest()
        {
            var expected = new SlimModel();

            var target = new DefaultTypeCreator();

            var actual = target.Populate(expected, null);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void SetsDefaultConfigurationForCreatorsTest()
        {
            var target = new DummyTypeCreator();

            target.AutoDetectConstructor.Should().BeTrue();
            target.AutoPopulate.Should().BeTrue();
            target.Priority.Should().Be(0);
        }

        [Fact]
        public void VerifyCreateRequestThrowsExceptionWithNullTypeTest()
        {
            var target = new DummyTypeCreator();

            Action action = () => target.VerifyWithNullType();

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}