using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DefaultBuildLogTests
    {
        [Fact]
        public void ClearRemovesExistingBuildLogDataTest()
        {
            var target = new DefaultBuildLog();

            target.CreatingValue(typeof(string), null);

            target.Output.Should().NotBeNullOrWhiteSpace();

            target.Clear();

            target.Output.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedTypeAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreatedType(typeof(string), null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatedType(null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateParameterAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreateParameter(typeof(Person), typeof(string), "FirstName", null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, typeof(string), "FirstName")]
        [InlineData(typeof(Person), null, "FirstName")]
        [InlineData(typeof(Person), typeof(string), null)]
        public void CreateParameterValidatesParametersTest(Type instanceType, Type parameterType, string parameterName)
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreateParameter(instanceType, parameterType, parameterName, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatePropertyAppendsLogEntryTest()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            target.CreateProperty(typeof(string), "FirstName", context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, "FirstName", true)]
        [InlineData(typeof(string), null, true)]
        [InlineData(typeof(string), "FirstName", false)]
        public void CreatePropertyValidatesPropertysTest(Type propertyType, string propertyName, bool includeContext)
        {
            Person context = null;

            if (includeContext)
            {
                context = new Person();
            }

            var target = new DefaultBuildLog();

            Action action = () => target.CreateProperty(propertyType, propertyName, context);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeIndentsChildMessagesTest()
        {
            var target = new DefaultBuildLog();

            target.CreatingType(typeof(Person), null);
            target.CreatingValue(typeof(string), null);
            target.CreatedType(typeof(Person), null);

            var actual = target.Output;

            actual.Should().Contain("    ");
        }

        [Fact]
        public void CreatingTypeAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreatingType(typeof(string), null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatingType(null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreatingValue(typeof(string), null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullValueTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatingValue(null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulatedInstanceAppendsLogEntryTest()
        {
            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatedInstance(instance);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatedInstanceThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.PopulatedInstance(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateInstanceIndentsChildMessagesTest()
        {
            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatingInstance(instance);
            target.CreatingValue(typeof(string), null);
            target.PopulatedInstance(instance);

            var actual = target.Output;

            actual.Should().Contain("    ");
        }

        [Fact]
        public void PopulatingInstanceAppendsLogEntryTest()
        {
            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatingInstance(instance);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatingInstanceThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.PopulatingInstance(null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}