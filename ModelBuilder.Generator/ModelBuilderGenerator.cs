namespace ModelBuilder.Generator
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ModelBuilderGenerator" /> class
    ///     is the incremental source generator that discovers <c>Model.Create&lt;T&gt;()</c> roots and
    ///     emits a builder for every concrete type reachable from them, plus the registration that wires
    ///     the builders into the runtime slots and registry.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class ModelBuilderGenerator : IIncrementalGenerator
    {
        private const string ModelTypeName = "ModelBuilder.vNext.Model";

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var roots = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => IsCandidateInvocation(node),
                    static (ctx, token) => GetRootType(ctx, token))
                .Where(static capture => capture.Symbol is not null)
                .Select(static (capture, _) => capture);

            var collected = roots.Collect();

            context.RegisterSourceOutput(collected, static (spc, captures) => Execute(spc, captures));
        }

        private static void Execute(SourceProductionContext context, ImmutableArray<RootCapture> captures)
        {
            var distinct = new Dictionary<string, INamedTypeSymbol>();

            foreach (var capture in captures)
            {
                if (capture.Symbol is null)
                {
                    continue;
                }

                ReportRootDiagnostics(context, capture);

                distinct[capture.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = capture.Symbol;
            }

            if (distinct.Count == 0)
            {
                return;
            }

            var models = BuildGraphWalker.Walk(distinct.Values);

            if (models.IsEmpty)
            {
                return;
            }

            context.AddSource("ModelBuilderGenerated.g.cs", SourceEmitter.Emit(models));
        }

        private static void ReportRootDiagnostics(SourceProductionContext context, RootCapture capture)
        {
            var symbol = capture.Symbol!;
            var location = capture.Location ?? Location.None;

            if (symbol.TypeKind == TypeKind.Interface || symbol.IsAbstract)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.UnmappedAbstractRoot,
                        location,
                        symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

                return;
            }

            if ((symbol.TypeKind == TypeKind.Class || symbol.TypeKind == TypeKind.Struct)
                && BuildGraphWalker.HasAccessibleConstructor(symbol) == false)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.NoAccessibleConstructor,
                        location,
                        symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }
        }

        private static RootCapture GetRootType(GeneratorSyntaxContext context, CancellationToken token)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Expression is not MemberAccessExpressionSyntax)
            {
                return default;
            }

            if (context.SemanticModel.GetSymbolInfo(invocation, token).Symbol is not IMethodSymbol method)
            {
                return default;
            }

            var containingType = method.ContainingType?.ToDisplayString();

            if (containingType != ModelTypeName)
            {
                return default;
            }

            if (method.Name != "Create" && method.Name != "Populate")
            {
                return default;
            }

            if (method.TypeArguments.Length != 1)
            {
                return default;
            }

            return new RootCapture(method.TypeArguments[0] as INamedTypeSymbol, invocation.GetLocation());
        }

        private static bool IsCandidateInvocation(SyntaxNode node)
        {
            return node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Name.Identifier.ValueText: "Create" or "Populate"
                }
            };
        }

        private readonly struct RootCapture
        {
            public RootCapture(INamedTypeSymbol? symbol, Location location)
            {
                Symbol = symbol;
                Location = location;
            }

            public Location? Location { get; }

            public INamedTypeSymbol? Symbol { get; }
        }
    }
}
