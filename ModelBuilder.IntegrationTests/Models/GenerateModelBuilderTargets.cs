// The [assembly: ...] attribute below must precede the namespace declaration. It names
// AssemblyLevelGenerateModelBuilderTarget as a build root purely through the assembly-level
// GenerateModelBuilder form - that type is never named in a Model.Create<T>()/Populate<T>()/
// Mapping<,>() call anywhere in this assembly, so a generated builder for it only exists because
// the generator discovered this attribute.
[assembly: ModelBuilder.GenerateModelBuilder(typeof(ModelBuilder.IntegrationTests.Models.AssemblyLevelGenerateModelBuilderTarget))]

namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A type only ever built via <c>Model.Create(typeof(TypeLevelGenerateModelBuilderTarget))</c> in
    ///     tests - never through a generic <c>Model.Create&lt;T&gt;()</c> call anywhere in this assembly.
    ///     The type-level <see cref="GenerateModelBuilderAttribute" /> is what makes the generator emit a
    ///     builder for it.
    /// </summary>
    [GenerateModelBuilder]
    public sealed class TypeLevelGenerateModelBuilderTarget
    {
        public string? Name { get; set; }

        public GenerateModelBuilderTargetDetail? Detail { get; set; }
    }

    /// <summary>
    ///     A member type reachable only through <see cref="TypeLevelGenerateModelBuilderTarget" />, used to
    ///     verify that types reachable from an attribute-discovered root also get a generated builder.
    /// </summary>
    public sealed class GenerateModelBuilderTargetDetail
    {
        public int Version { get; set; }
    }

    /// <summary>
    ///     A type only ever built via <c>Model.Create(typeof(AssemblyLevelGenerateModelBuilderTarget))</c>
    ///     in tests. It carries no attribute of its own; the assembly-level
    ///     <c>[assembly: GenerateModelBuilder(typeof(AssemblyLevelGenerateModelBuilderTarget))]</c>
    ///     declared at the top of this file is what makes the generator emit a builder for it.
    /// </summary>
    public sealed class AssemblyLevelGenerateModelBuilderTarget
    {
        public string? Reference { get; set; }
    }
}
