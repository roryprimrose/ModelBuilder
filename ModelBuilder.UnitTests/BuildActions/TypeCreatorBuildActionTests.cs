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
        public void BuildForParameterInfoReturnsCreatorValueWhenMatchingCreatorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            executeStrategy.Configuration.Returns(buildConfiguration);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy, arguments)
                .Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo, arguments);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoCreatorsExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoMatchingCreatorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsValueFromCreatorWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstCreator);
            buildConfiguration.TypeCreators.Add(secondCreator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstCreator.Priority.Returns(10);
            firstCreator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(true);
            secondCreator.Priority.Returns(20);
            secondCreator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(true);
            secondCreator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy, arguments)
                .Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo, arguments);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(null, parameterInfo, arguments);

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
        public void BuildForPropertyInfoReturnsCreatorValueWhenMatchingCreatorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy, arguments).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo, arguments);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoCreatorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingCreatorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromCreatorWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstCreator);
            buildConfiguration.TypeCreators.Add(secondCreator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstCreator.Priority.Returns(10);
            firstCreator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(true);
            secondCreator.Priority.Returns(20);
            secondCreator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(true);
            secondCreator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy, arguments)
                .Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo, arguments);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(null, propertyInfo, arguments);

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
        public void BuildForTypeReturnsCreatorValueWhenMatchingCreatorFound()
        {
            var type = typeof(Person);
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(type, null, buildConfiguration, buildChain).Returns(true);
            creator.Create(type, null, executeStrategy, arguments).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type, arguments);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoCreatorsExist()
        {
            var type = typeof(Person);
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingCreatorFound()
        {
            var type = typeof(Person);
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type, arguments);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueFromCreatorWithHighestPriority()
        {
            var type = typeof(Person);
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstCreator);
            buildConfiguration.TypeCreators.Add(secondCreator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstCreator.Priority.Returns(10);
            firstCreator.CanCreate(type, null, buildConfiguration, buildChain).Returns(true);
            secondCreator.Priority.Returns(20);
            secondCreator.CanCreate(type, null, buildConfiguration, buildChain).Returns(true);
            secondCreator.Create(type, null, executeStrategy, arguments).Returns(expected);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Build(executeStrategy, type, arguments);

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
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy, arguments)
                .Throws<BuildException>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, parameterInfo, arguments);

            var exception = action.Should().Throw<BuildException>().Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void BuildThrowsExceptionWhenCreatorFails()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy, arguments)
                .Throws<TimeoutException>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Build(executeStrategy, propertyInfo, arguments);

            var exception = action.Should().Throw<BuildException>().Which;

            exception.InnerException.Should().BeOfType<TimeoutException>();

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoCreatorsExist()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoMatchingCreatorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (ParameterInfo) null);

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
        public void GetBuildCapabilityForParameterReturnsMatchResultFromTypeCreator(bool canPopulate,
            bool autoDetectConstructor,
            bool autoPopulate)
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.CanPopulate(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain).Returns(canPopulate);
            creator.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy, arguments)
                .Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
            actual.ImplementedByType.Should().Be(creator.GetType());
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoCreatorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoMatchingCreatorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (PropertyInfo) null);

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
        public void GetBuildCapabilityForPropertyReturnsMatchResultFromTypeCreator(bool canPopulate,
            bool autoDetectConstructor,
            bool autoPopulate)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(true);
            creator.CanPopulate(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain).Returns(canPopulate);
            creator.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy, arguments).Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
            actual.ImplementedByType.Should().Be(creator.GetType());
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
        public void GetBuildCapabilityForTypeReturnsMatchResultFromTypeCreator(bool canPopulate,
            bool autoDetectConstructor,
            bool autoPopulate)
        {
            var type = typeof(Person);
            var arguments = new object[] {Guid.NewGuid().ToString()};
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(type, null, buildConfiguration, buildChain).Returns(true);
            creator.CanPopulate(type, null, buildConfiguration, buildChain).Returns(canPopulate);
            creator.Create(type, null, executeStrategy).Returns(expected);
            creator.AutoDetectConstructor.Returns(autoDetectConstructor);
            creator.AutoPopulate.Returns(autoPopulate);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().Be(canPopulate);
            actual.AutoDetectConstructor.Should().Be(autoDetectConstructor);
            actual.AutoPopulate.Should().Be(autoPopulate);
            actual.ImplementedByType.Should().Be(creator.GetType());
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoCreatorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoMatchingCreatorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateReturnsCreatorValueWhenMatchingCreatorFound()
        {
            var instance = new Person();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(creator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            creator.CanCreate(instance.GetType(), null, buildConfiguration, buildChain).Returns(true);
            creator.Populate(instance, executeStrategy).Returns(instance);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Populate(executeStrategy, instance);

            actual.Should().Be(instance);

            creator.Received().Populate(instance, executeStrategy);
        }

        [Fact]
        public void PopulateReturnsNullWhenNoCreatorsExist()
        {
            var instance = new Person();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Populate(executeStrategy, instance);

            actual.Should().Be(instance);
        }

        [Fact]
        public void PopulateReturnsValueFromCreatorWithHighestPriority()
        {
            var instance = new Person();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();

            buildConfiguration.TypeCreators.Add(firstCreator);
            buildConfiguration.TypeCreators.Add(secondCreator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstCreator.Priority.Returns(10);
            firstCreator.CanCreate(instance.GetType(), null, buildConfiguration, buildChain).Returns(true);
            secondCreator.Priority.Returns(20);
            secondCreator.CanCreate(instance.GetType(), null, buildConfiguration, buildChain).Returns(true);
            secondCreator.Populate(instance, executeStrategy).Returns(instance);

            var sut = new TypeCreatorBuildAction();

            var actual = sut.Populate(executeStrategy, instance);

            actual.Should().Be(instance);

            secondCreator.Received().Populate(instance, executeStrategy);
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategy()
        {
            var instance = new Person();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Populate(null, instance);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new TypeCreatorBuildAction();

            Action action = () => sut.Populate(executeStrategy, null);

            action.Should().Throw<ArgumentNullException>();
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