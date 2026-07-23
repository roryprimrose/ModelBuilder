namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests proving the <c>[GenerateModelBuilder]</c> attribute - both the type-level and
    ///     assembly-level forms - causes the generator to emit a builder for a type that is never named in
    ///     a <c>Model.Create&lt;T&gt;()</c>/<c>Populate&lt;T&gt;()</c>/<c>Mapping&lt;,&gt;()</c> call
    ///     anywhere in this assembly. Without attribute discovery, <c>Model.Create(typeof(X))</c> for these
    ///     types would throw <see cref="ModelBuildException" /> because the registry would have no builder
    ///     to dispatch to.
    /// </summary>
    public class GenerateModelBuilderAttributeDiscoveryTests
    {
        [Fact]
        public void CreateBuildsTypeAnnotatedWithTypeLevelAttribute()
        {
            var actual = (TypeLevelGenerateModelBuilderTarget)Model.Create(
                typeof(TypeLevelGenerateModelBuilderTarget));

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateBuildsTypeReachableFromTypeLevelAttributeRoot()
        {
            var actual = (TypeLevelGenerateModelBuilderTarget)Model.Create(
                typeof(TypeLevelGenerateModelBuilderTarget));

            actual.Detail.Should().NotBeNull();
            actual.Detail!.Version.Should().NotBe(0);
        }

        [Fact]
        public void CreateBuildsTypeNamedByAssemblyLevelAttribute()
        {
            var actual = (AssemblyLevelGenerateModelBuilderTarget)Model.Create(
                typeof(AssemblyLevelGenerateModelBuilderTarget));

            actual.Should().NotBeNull();
            actual.Reference.Should().NotBeNullOrWhiteSpace();
        }
    }
}
