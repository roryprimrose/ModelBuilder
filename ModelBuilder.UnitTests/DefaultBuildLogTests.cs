namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class DefaultBuildLogTests
    {
        [Fact]
        public void BuildFailureAppendsLogEntryTest()
        {
            var ex = new BuildException(Guid.NewGuid().ToString());

            var target = new DefaultBuildLog();

            target.BuildFailure(ex);

            target.Output.Should().Contain(ex.ToString());
        }

        [Fact]
        public void BuildFailureThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.BuildFailure(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CircularReferenceDetectedAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CircularReferenceDetected(typeof(string));

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CircularReferenceDetectedThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CircularReferenceDetected(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ClearRemovesExistingBuildLogDataTest()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var target = new DefaultBuildLog();

            target.CreatingValue(type, generatorType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();

            target.Clear();

            target.Output.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedParameterAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreatedParameter(typeof(Person), typeof(string), "FirstName", null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, typeof(string), "FirstName")]
        [InlineData(typeof(Person), null, "FirstName")]
        [InlineData(typeof(Person), typeof(string), null)]
        public void CreatedParameterValidatesParametersTest(Type instanceType, Type parameterType, string parameterName)
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatedParameter(instanceType, parameterType, parameterName, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatedPropertyAppendsLogEntryTest()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            target.CreatedProperty(typeof(string), "FirstName", context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, "FirstName", true)]
        [InlineData(typeof(string), null, true)]
        [InlineData(typeof(string), "FirstName", false)]
        public void CreatedPropertyValidatesPropertysTest(Type propertyType, string propertyName, bool includeContext)
        {
            Person context = null;

            if (includeContext)
            {
                context = new Person();
            }

            var target = new DefaultBuildLog();

            Action action = () => target.CreatedProperty(propertyType, propertyName, context);

            action.ShouldThrow<ArgumentNullException>();
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
        public void CreateTypeIndentsChildMessagesTest()
        {
            var generatorType = typeof(StateValueGenerator);
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            target.CreatingType(typeof(Person), creatorType, null);
            target.CreatingValue(typeof(string), generatorType, null);
            target.CreatedType(typeof(Person), null);

            var actual = target.Output;

            actual.Should().Contain("    ");
        }

        [Fact]
        public void CreatingParameterAppendsLogEntryTest()
        {
            var target = new DefaultBuildLog();

            target.CreatingParameter(typeof(Person), typeof(string), "FirstName", null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, typeof(string), "FirstName")]
        [InlineData(typeof(Person), null, "FirstName")]
        [InlineData(typeof(Person), typeof(string), null)]
        public void CreatingParameterValidatesParametersTest(
            Type instanceType,
            Type parameterType,
            string parameterName)
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatingParameter(instanceType, parameterType, parameterName, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingPropertyAppendsLogEntryTest()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            target.CreatingProperty(typeof(string), "FirstName", context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, "FirstName", true)]
        [InlineData(typeof(string), null, true)]
        [InlineData(typeof(string), "FirstName", false)]
        public void CreatingPropertyValidatesPropertysTest(Type propertyType, string propertyName, bool includeContext)
        {
            Person context = null;

            if (includeContext)
            {
                context = new Person();
            }

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingProperty(propertyType, propertyName, context);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeAppendsLogEntryTest()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            target.CreatingType(typeof(string), creatorType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullCreatorTypeTest()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingType(type, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullTypeTest()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingType(null, creatorType, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueAppendsLogEntryTest()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var target = new DefaultBuildLog();

            target.CreatingValue(type, generatorType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullGeneratorTypeTest()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingValue(type, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullTypeTest()
        {
            var generatorType = typeof(StateValueGenerator);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingValue(null, generatorType, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringPropertyAppendsLogEntryTest()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            target.IgnoringProperty(typeof(string), "FirstName", context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, "FirstName", true)]
        [InlineData(typeof(string), null, true)]
        [InlineData(typeof(string), "FirstName", false)]
        public void IgnoringPropertyValidatesPropertysTest(
            Type propertyType,
            string propertyName,
            bool includeContext)
        {
            Person context = null;

            if (includeContext)
            {
                context = new Person();
            }

            var target = new DefaultBuildLog();

            Action action = () => target.IgnoringProperty(propertyType, propertyName, context);

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
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatingInstance(instance);
            target.CreatingValue(type, generatorType, null);
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

        [Fact]
        public void PostBuildActionAppendsLogEntryTest()
        {
            var postBuildType = typeof(DummyPostBuildAction);

            var target = new DefaultBuildLog();

            target.PostBuildAction(typeof(string), postBuildType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullPostBuildTypeTest()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.PostBuildAction(type, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullTypeTest()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Action action = () => target.PostBuildAction(null, creatorType, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void WritingLogEntryIndentsAllLinesOfMessageTest()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Exception exception;

            try
            {
                throw new TimeoutException();
            }
            catch (Exception ex)
            {
                // Get the exception with a valid stack trace
                exception = ex;
            }

            target.CreatingType(typeof(Person), creatorType, null);
            target.BuildFailure(exception);
            target.CreatedType(typeof(Person), null);

            var lines = target.Output.Split(
                new[]
                {
                    Environment.NewLine
                },
                StringSplitOptions.RemoveEmptyEntries);
            var indentedLines = lines.Skip(1).Take(lines.Length - 2);

            indentedLines.All(x => x.StartsWith("    ")).Should().BeTrue();
        }
    }
}