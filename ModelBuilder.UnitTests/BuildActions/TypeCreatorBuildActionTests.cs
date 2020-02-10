namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;
    using Xunit.Abstractions;

    public class TypeCreatorBuildActionTests
    {
        private readonly OutputBuildLog _buildLog;
        private readonly ITestOutputHelper _output;

        public TypeCreatorBuildActionTests(ITestOutputHelper output)
        {
            _output = output;
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void BuildForParameterInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            executeStrategy.Configuration.Returns(buildConfiguration);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoGeneratorsExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsValueFromGeneratorWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<ITypeCreator>();
            var secondGenerator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstGenerator);
            buildConfiguration.TypeCreators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy)
                .Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoGeneratorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromGeneratorWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<ITypeCreator>();
            var secondGenerator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstGenerator);
            buildConfiguration.TypeCreators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(type, null, buildChain).Returns(true);
            creator.Create(type, null, executeStrategy).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoGeneratorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueFromGeneratorWithHighestPriority()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<ITypeCreator>();
            var secondGenerator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstGenerator);
            buildConfiguration.TypeCreators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.CanCreate(type, null, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.CanCreate(type, null, buildChain).Returns(true);
            secondGenerator.Create(type, null, executeStrategy).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildRethrowsBuildException()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy)
                .Throws<BuildException>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, parameterInfo);

            var exception = action.Should().Throw<BuildException>().Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void BuildThrowsExceptionWhenGeneratorFails()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy)
                .Throws<TimeoutException>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, propertyInfo);

            var exception = action.Should().Throw<BuildException>().Which;

            exception.InnerException.Should().BeOfType<TimeoutException>();

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            creator.CanPopulate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(true);
            creator.AutoPopulate.Returns(true);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeTrue();
            actual.AutoDetectConstructor.Should().BeTrue();
            actual.AutoPopulate.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoGeneratorsExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(null, buildChain, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        public void IsMatchForParameterReturnsMatchResultFromTypeCreator(bool canPopulate, bool autoDetectConstructor,
            bool autoPopulate)
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            creator.CanPopulate(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(canPopulate);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(true);
            creator.AutoPopulate.Returns(true);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeTrue();
            actual.AutoDetectConstructor.Should().BeTrue();
            actual.AutoPopulate.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoGeneratorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(null, buildChain, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        public void IsMatchForPropertyReturnsMatchResultFromTypeCreator(bool canPopulate, bool autoDetectConstructor,
            bool autoPopulate)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            creator.CanPopulate(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(canPopulate);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        public void IsMatchForTypeReturnsMatchResultFromTypeCreator(bool canPopulate, bool autoDetectConstructor,
            bool autoPopulate)
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(type, null, buildChain).Returns(true);
            creator.CanPopulate(type, null, buildChain).Returns(canPopulate);
            creator.Create(type, null, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoGeneratorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.IsMatch(buildConfiguration, buildChain, type);

            actual.SupportsCreate.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(null, buildChain, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.IsMatch(buildConfiguration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsException()
        {
            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Populate(null, null);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsLowerThanCreationRuleBuildActionPriority()
        {
            var otherStep = new CreationRuleBuildAction();

            var sut = new TypeCreatorBuildAction();

            sut.Priority.Should().BeLessThan(otherStep.Priority);
        }
    }
}