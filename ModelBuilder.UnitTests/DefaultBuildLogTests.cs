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

            var sut = new DefaultBuildLog();

            sut.BuildFailure(ex);

            sut.Output.Should().Contain(ex.ToString());
        }

        [Fact]
        public void BuildFailureThrowsExceptionWithNullType()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.BuildFailure(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CircularReferenceDetectedAppendsLogEntry()
        {
            var sut = new DefaultBuildLog();

            sut.CircularReferenceDetected(typeof(string));

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CircularReferenceDetectedThrowsExceptionWithNullType()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.CircularReferenceDetected(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ClearRemovesExistingBuildLogData()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var sut = new DefaultBuildLog();

            sut.CreatingValue(type, generatorType, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();

            sut.Clear();

            sut.Output.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedParameterAppendsLogEntry()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new DefaultBuildLog();

            sut.CreatedParameter(parameterInfo, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedParameterThrowsExceptionWithNullParameter()
        {
            var context = Guid.NewGuid().ToString();

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatedParameter(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var sut = new DefaultBuildLog();

            sut.CreatedProperty(propertyInfo, context);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatedProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatedProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatedTypeAppendsLogEntry()
        {
            var sut = new DefaultBuildLog();

            sut.CreatedType(typeof(string), null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatedTypeThrowsExceptionWithNullType()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatedType(null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeIndentsChildMessages()
        {
            var generatorType = typeof(StateValueGenerator);
            var creatorType = typeof(DefaultTypeCreator);

            var sut = new DefaultBuildLog();

            sut.CreatingType(typeof(Person), creatorType, null);
            sut.CreatingValue(typeof(string), generatorType, null);
            sut.CreatedType(typeof(Person), null);

            var actual = sut.Output;

            actual.Should().Contain("    ");
        }

        [Fact]
        public void CreatingParameterAppendsLogEntry()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new DefaultBuildLog();

            sut.CreatingParameter(parameterInfo, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingParameterThrowsExceptionWithNullParameter()
        {
            var context = Guid.NewGuid().ToString();

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingParameter(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var sut = new DefaultBuildLog();

            sut.CreatingProperty(propertyInfo, context);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeAppendsLogEntry()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var sut = new DefaultBuildLog();

            sut.CreatingType(typeof(string), creatorType, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullCreatorType()
        {
            var type = typeof(string);

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingType(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingTypeThrowsExceptionWithNullType()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingType(null, creatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueAppendsLogEntry()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var sut = new DefaultBuildLog();

            sut.CreatingValue(type, generatorType, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullGeneratorType()
        {
            var type = typeof(string);

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingValue(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatingValueThrowsExceptionWithNullType()
        {
            var generatorType = typeof(StateValueGenerator);

            var sut = new DefaultBuildLog();

            Action action = () => sut.CreatingValue(null, generatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringPropertyAppendsLogEntry()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var context = new Person();

            var sut = new DefaultBuildLog();

            sut.IgnoringProperty(propertyInfo, context);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void IgnoringPropertyThrowsExceptionWithNullProperty()
        {
            var context = new Person();

            var sut = new DefaultBuildLog();

            Action action = () => sut.IgnoringProperty(null, context);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringPropertyThrowsExceptionWithNullContext()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new DefaultBuildLog();

            Action action = () => sut.IgnoringProperty(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappedTypeAppendsLogEntry()
        {
            var sut = new DefaultBuildLog();

            sut.MappedType(typeof(Stream), typeof(MemoryStream));

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void MappedTypeThrowsExceptionWithNullSourceType()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.MappedType(null, typeof(MemoryStream));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappedTypeThrowsExceptionWithNullTargetType()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.MappedType(typeof(Stream), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatedInstanceAppendsLogEntry()
        {
            var instance = new Person();

            var sut = new DefaultBuildLog();

            sut.PopulatedInstance(instance);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatedInstanceThrowsExceptionWithNullInstance()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.PopulatedInstance(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateInstanceIndentsChildMessages()
        {
            var generatorType = typeof(StateValueGenerator);
            var type = typeof(string);

            var instance = new Person();

            var sut = new DefaultBuildLog();

            sut.PopulatingInstance(instance);
            sut.CreatingValue(type, generatorType, null);
            sut.PopulatedInstance(instance);

            var actual = sut.Output;

            actual.Should().Contain("    ");
        }

        [Fact]
        public void PopulatingInstanceAppendsLogEntry()
        {
            var instance = new Person();

            var sut = new DefaultBuildLog();

            sut.PopulatingInstance(instance);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulatingInstanceThrowsExceptionWithNullInstance()
        {
            var sut = new DefaultBuildLog();

            Action action = () => sut.PopulatingInstance(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PostBuildActionAppendsLogEntry()
        {
            var postBuildType = typeof(DummyPostBuildAction);

            var sut = new DefaultBuildLog();

            sut.PostBuildAction(typeof(string), postBuildType, null);

            sut.Output.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullPostBuildType()
        {
            var type = typeof(string);

            var sut = new DefaultBuildLog();

            Action action = () => sut.PostBuildAction(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PostBuildActionThrowsExceptionWithNullType()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var sut = new DefaultBuildLog();

            Action action = () => sut.PostBuildAction(null, creatorType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WritingLogEntryIndentsAllLinesOfMessage()
        {
            var creatorType = typeof(DefaultTypeCreator);

            var sut = new DefaultBuildLog();

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

            sut.CreatingType(typeof(Person), creatorType, null);
            sut.BuildFailure(exception);
            sut.CreatedType(typeof(Person), null);

            var lines = sut.Output.Split(
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