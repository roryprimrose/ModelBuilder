namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class TypeCreatorBaseTests
    {
        [Theory]
        [InlineData(typeof(BuildStrategyBase), false)]
        [InlineData(typeof(IBuildStrategy), false)]
        [InlineData(typeof(Environment), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(MemoryStream), true)]
        [InlineData(typeof(List<Person>), true)]
        [InlineData(typeof(KeyValuePair<string, Person>), true)]
        public void CanCreateReturnsWhetherTypeCanBeCreatedTest(Type targetType, bool expected)
        {
            var target = new DefaultTypeCreator();

            var actual = target.CanCreate(targetType, null, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultTypeCreator();

            Action action = () => target.CanCreate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateReturnsTrueTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.CanPopulate(typeof(Person), null, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();

            var target = new DefaultTypeCreator();

            Action action = () => target.CanPopulate(null, "Name", buildChain);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateDoesNotThrowsExceptionWhenCreateVerificationPassesTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(typeof(List<string>), null, executeStrategy);

            action.ShouldNotThrow();
        }

        [Fact]
        public void CreateThrowsExceptionWhenCreateVerificationFailsTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(typeof(bool), null, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(null, "Name", executeStrategy);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsInstanceTest()
        {
            var target = new TypeCreatorWrapper();

            target.Random.Should().NotBeNull();
        }

        [Fact]
        public void PopulateDoesNotThrowsExceptionWhenPopulateVerificationPassesTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var value = new List<string>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(value, executeStrategy);

            action.ShouldNotThrow();
        }

        [Fact]
        public void PopulateReturnsProvidedInstanceTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var expected = new SlimModel();

            var target = new DefaultTypeCreator();

            var actual = target.Populate(expected, executeStrategy);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenPopulateVerificationFailsTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(false, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategyTest()
        {
            var person = new Person();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(person, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(null, executeStrategy);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DefaultTypeCreator();

            Action action = () => target.Populate(null, executeStrategy);

            action.ShouldThrow<ArgumentNullException>();
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
        public void VerifyCreateRequestThrowsExceptionWithNullExecuteStrategyTest()
        {
            var target = new DummyTypeCreator();

            Action action = () => target.VerifyCreateRequestWithNullExecuteStrategy();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void VerifyCreateRequestThrowsExceptionWithNullTypeTest()
        {
            var target = new DummyTypeCreator();

            Action action = () => target.VerifyCreateRequestWithNullType();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void VerifyPopulateRequestThrowsExceptionWithNullExecuteStrategyTest()
        {
            var target = new DummyTypeCreator();

            Action action = () => target.VerifyPopulateRequestWithNullExecuteStrategy();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void VerifyPopulateRequestThrowsExceptionWithNullTypeTest()
        {
            var target = new DummyTypeCreator();

            Action action = () => target.VerifyPopulateRequestWithNullType();

            action.ShouldThrow<ArgumentNullException>();
        }

        private class TypeCreatorWrapper : TypeCreatorBase
        {
            public override bool CanCreate(Type type, string referenceName, LinkedList<object> buildChain)
            {
                var canCreate = base.CanCreate(type, referenceName, buildChain);

                if (canCreate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            public override bool CanPopulate(Type type, string referenceName, LinkedList<object> buildChain)
            {
                var canPopulate = base.CanPopulate(type, referenceName, buildChain);

                if (canPopulate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            protected override object CreateInstance(
                Type type,
                string referenceName,
                IExecuteStrategy executeStrategy,
                params object[] args)
            {
                return new List<string>();
            }

            protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
            {
                return instance;
            }

            public IRandomGenerator Random => Generator;
        }
    }
}