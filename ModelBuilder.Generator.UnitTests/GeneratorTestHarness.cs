namespace ModelBuilder.Generator.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    ///     The <see cref="GeneratorTestHarness" /> class
    ///     runs the <see cref="ModelBuilderGenerator" /> over in-memory source and exposes the generator
    ///     diagnostics, generated sources, the resulting compilation diagnostics, and an emitted
    ///     assembly for execution tests.
    /// </summary>
    internal sealed class GeneratorTestHarness
    {
        private static readonly ImmutableArray<MetadataReference> _references = BuildReferences();

        private GeneratorTestHarness(
            ImmutableArray<Diagnostic> generatorDiagnostics,
            ImmutableArray<string> generatedSources,
            ImmutableArray<Diagnostic> compilationDiagnostics,
            CSharpCompilation outputCompilation)
        {
            GeneratorDiagnostics = generatorDiagnostics;
            GeneratedSources = generatedSources;
            CompilationDiagnostics = compilationDiagnostics;
            OutputCompilation = outputCompilation;
        }

        public static GeneratorTestHarness Run(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                source,
                new CSharpParseOptions(LanguageVersion.Latest));

            var compilation = CSharpCompilation.Create(
                "GeneratorTestAssembly",
                new[] { syntaxTree },
                _references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

            var driver = CSharpGeneratorDriver.Create(new ModelBuilderGenerator());

            driver.RunGeneratorsAndUpdateCompilation(
                    compilation,
                    out var outputCompilation,
                    out var generatorDiagnostics)
                .GetRunResult();

            var generatedSources = outputCompilation.SyntaxTrees
                .Where(tree => ReferenceEquals(tree, syntaxTree) == false)
                .Select(tree => tree.ToString())
                .ToImmutableArray();

            return new GeneratorTestHarness(
                generatorDiagnostics,
                generatedSources,
                outputCompilation.GetDiagnostics(),
                (CSharpCompilation)outputCompilation);
        }

        public Assembly EmitAndLoad()
        {
            using var stream = new MemoryStream();

            var result = OutputCompilation.Emit(stream);

            if (result.Success == false)
            {
                var errors = string.Join(
                    Environment.NewLine,
                    result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

                throw new InvalidOperationException("Generated code failed to compile:" + Environment.NewLine + errors);
            }

            stream.Seek(0, SeekOrigin.Begin);

            var assembly = Assembly.Load(stream.ToArray());

            System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(
                assembly.Modules.First().ModuleHandle);

            return assembly;
        }

        public ImmutableArray<Diagnostic> CompilationErrors =>
            CompilationDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();

        public ImmutableArray<Diagnostic> CompilationDiagnostics { get; }

        public ImmutableArray<string> GeneratedSources { get; }

        public ImmutableArray<Diagnostic> GeneratorDiagnostics { get; }

        public CSharpCompilation OutputCompilation { get; }

        private static ImmutableArray<MetadataReference> BuildReferences()
        {
            var references = new List<MetadataReference>();

            var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");

            if (trusted != null)
            {
                foreach (var path in trusted.Split(Path.PathSeparator))
                {
                    if (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        references.Add(MetadataReference.CreateFromFile(path));
                    }
                }
            }

            references.Add(MetadataReference.CreateFromFile(typeof(ModelBuilder.Model).Assembly.Location));

            return references.ToImmutableArray();
        }
    }
}
