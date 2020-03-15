namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class DefaultBuildLogTests
    {
        [Fact]
        public void BuildFailureAppendsLogEntry()
        {
            var ex = new BuildException(Guid.NewGuid().ToString());

            var target = new DefaultBuildLog();

            target.BuildFailure(ex);

            target.Output.Should().Contain(ex.ToString());
        }

        [Fact]
        public void BuildFailureThrowsExceptionWithNullType()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.BuildFailure(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CircularReferenceDetectedAppendsLogEntry()
        {
            var target = new DefaultBuildLog();

            target.CircularReferenceDetected(typeof(string));

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CircularReferenceDetectedThrowsExceptionWithNullType()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CircularReferenceDetected(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ClearRemovesExistingBuildLogData()
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
        public void CreatedParameterAppendsLogEntry()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var target = new DefaultBuildLog();

            target.CreatedParameter(parameterInfo, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedParameterThrowsExceptionWithNullParameter()
        {
            var context = Guid.NewGuid().ToString();

            var target = new DefaultBuildLog();

            Action action = () => target.CreatedParameter(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var target = new DefaultBuildLog();

            target.CreatedProperty(propertyInfo, context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new DefaultBuildLog();

            Action action = () => target.CreatedProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            Action action = () => target.CreatedProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedTypeAppendsLogEntry()
        {
            var target = new DefaultBuildLog();

            target.CreatedType(typeof(string), null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedTypeThrowsExceptionWithNullType()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.CreatedType(null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeIndentsChildMessages()
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
        public void CreatingParameterAppendsLogEntry()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var target = new DefaultBuildLog();

            target.CreatingParameter(parameterInfo, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingParameterThrowsExceptionWithNullParameter()
        {
            var context = Guid.NewGuid().ToString();

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingParameter(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var target = new DefaultBuildLog();

            target.CreatingProperty(propertyInfo, context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeAppendsLogEntry()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            target.CreatingType(typeof(string), creatorType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullCreatorType()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingType(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullType()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingType(null, creatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueAppendsLogEntry()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var target = new DefaultBuildLog();

            target.CreatingValue(type, generatorType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullGeneratorType()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingValue(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullType()
        {
            var generatorType = typeof(StateValueGenerator);

            var target = new DefaultBuildLog();

            Action action = () => target.CreatingValue(null, generatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var target = new DefaultBuildLog();

            target.IgnoringProperty(propertyInfo, context);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void IgnoringPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var target = new DefaultBuildLog();

            Action action = () => target.IgnoringProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new DefaultBuildLog();

            Action action = () => target.IgnoringProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappedTypeAppendsLogEntry()
        {
            var target = new DefaultBuildLog();

            target.MappedType(typeof(Stream), typeof(MemoryStream));

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void MappedTypeThrowsExceptionWithNullSourceType()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.MappedType(null, typeof(MemoryStream));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappedTypeThrowsExceptionWithNullTargetType()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.MappedType(typeof(Stream), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatedInstanceAppendsLogEntry()
        {
            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatedInstance(instance);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatedInstanceThrowsExceptionWithNullInstance()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.PopulatedInstance(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateInstanceIndentsChildMessages()
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
        public void PopulatingInstanceAppendsLogEntry()
        {
            var instance = new Person();

            var target = new DefaultBuildLog();

            target.PopulatingInstance(instance);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatingInstanceThrowsExceptionWithNullInstance()
        {
            var target = new DefaultBuildLog();

            Action action = () => target.PopulatingInstance(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PostBuildActionAppendsLogEntry()
        {
            var postBuildType = typeof(DummyPostBuildAction);

            var target = new DefaultBuildLog();

            target.PostBuildAction(typeof(string), postBuildType, null);

            target.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullPostBuildType()
        {
            var type = typeof(string);

            var target = new DefaultBuildLog();

            Action action = () => target.PostBuildAction(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullType()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Action action = () => target.PostBuildAction(null, creatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WritingLogEntryIndentsAllLinesOfMessage()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var target = new DefaultBuildLog();

            Exception exception;

            try
            {
                throw new TimeoutException();
            }
            catch (TimeoutException ex)
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

            indentedLines.All(x => x.StartsWith("    ", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }
    }
}