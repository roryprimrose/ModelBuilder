namespace ModelBuilder.Generator
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    ///     The <see cref="DiagnosticDescriptors" /> class
    ///     holds the diagnostics the generator reports, with stable <c>MB####</c> identifiers.
    /// </summary>
    internal static class DiagnosticDescriptors
    {
        private const string Category = "ModelBuilder";

        public static readonly DiagnosticDescriptor UnmappedAbstractRoot = new DiagnosticDescriptor(
            "MB1001",
            "Abstract or interface build root has no mapping",
            "'{0}' is abstract or an interface and has no Mapping<,> to a concrete type, so no builder can be generated. Add Model.Mapping<{0}, TConcrete>() or build a concrete type instead.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor NoAccessibleConstructor = new DiagnosticDescriptor(
            "MB1002",
            "Build root has no accessible constructor",
            "'{0}' has no public constructor accessible to the generated code, so no builder can be generated. Add a public constructor or expose an accessible one.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor UnbuildableTypeOfRoot = new DiagnosticDescriptor(
            "MB1005",
            "Model.Create(typeof(X)) names a type that cannot be built",
            "'{0}' has no generated builder because it is abstract, an interface, or has no accessible constructor. Make it discoverable: call Model.Create<{0}>() somewhere, add a Mapping<,> to it, or annotate it with [GenerateModelBuilder].",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor UnsupportedCollectionShape = new DiagnosticDescriptor(
            "MB1011",
            "Discovered collection shape is not supported",
            "'{0}' is a collection shape ModelBuilder does not build (it has no usable mutator, or it is a live view over another collection). Add a Model.Mapping<,> to a supported collection, or register a custom IValueSource<{0}>.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor OpenMappingTargetNoAccessibleConstructor = new DiagnosticDescriptor(
            "MB1006",
            "Open generic mapping target has no accessible constructor",
            "'{0}' has no public constructor accessible to the generated code, so no builder can ever be generated for a closed shape of the '{1}' mapping. Add a public constructor or expose an accessible one.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor OpenMappingNeverUsedInClosedForm = new DiagnosticDescriptor(
            "MB1007",
            "Open generic mapping is never used in closed form",
            "'{0}' has an open generic mapping registered, but no closed Model.Mapping<,>() call declares a shape to build, so no builder will ever be generated for it. Add Model.Mapping<TClosedSource, TClosedTarget>() for each closed shape you need built.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");

        public static readonly DiagnosticDescriptor UnmappedAbstractMember = new DiagnosticDescriptor(
            "MB1012",
            "Member resolves to an unmapped abstract or interface type",
            "'{0}' is abstract or an interface and appears as a member type with no Mapping<,> to a concrete type, so it will be left at its default value wherever it is used. Add Model.Mapping<{0}, TConcrete>() to give it a concrete type.",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/roryprimrose/ModelBuilder");
    }
}
