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
    }
}
