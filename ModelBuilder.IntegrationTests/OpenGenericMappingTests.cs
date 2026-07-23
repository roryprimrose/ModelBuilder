namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of open generic type mapping via
    ///     <c>Model.Mapping(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;))</c>, and of closed
    ///     constructed generic types building like any other class independent of any mapping.
    /// </summary>
    public class OpenGenericMappingTests
    {
        [Fact]
        public void CreatePopulatesClosedGenericInterfaceMemberUsingOpenGenericMapping()
        {
            var actual = Model.Mapping(typeof(IRepository<>), typeof(Repository<>))
                .Mapping<IRepository<Widget>, Repository<Widget>>()
                .Create<RepositoryHolder>();

            actual.Repository.Should().NotBeNull();
            actual.Repository.Should().BeOfType<Repository<Widget>>();
            actual.Repository!.Item.Should().NotBeNull();
            actual.Repository.Item!.Name.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreatePopulatesClosedGenericInterfaceMemberUsingOpenGenericMappingConfiguredByModule()
        {
            var actual = Model.UsingModule<IntegrationTestModule>().Create<RepositoryHolder>();

            actual.Repository.Should().NotBeNull();
            actual.Repository.Should().BeOfType<Repository<Widget>>();
        }

        [Fact]
        public void CreateBuildsClosedGenericTypeAsRootWithNoMappingRegistered()
        {
            var actual = Model.Create<Box<Widget>>();

            actual.Value.Should().NotBeNull();
            actual.Value!.Name.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateBuildsClosedGenericMemberWithNoMappingRegistered()
        {
            var actual = Model.Create<BoxHolder>();

            actual.Item.Should().NotBeNull();
            actual.Item!.Value.Should().NotBeNull();
            actual.Item.Value!.Name.Should().NotBeNullOrWhiteSpace();
        }
    }
}
