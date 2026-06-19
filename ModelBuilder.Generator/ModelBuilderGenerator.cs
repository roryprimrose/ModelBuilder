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
        private const string ModelTypeName = "ModelBuilder.Model";
        private const string ConfigurationTypeName = "ModelBuilder.IModelConfiguration";

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var roots = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => IsCandidateInvocation(node),
                    static (ctx, token) => GetRootType(ctx, token))
                .Where(static capture => capture.Symbol is not null)
                .Select(static (capture, _) => capture);

            var hasModuleInitializer = context.CompilationProvider.Select(
                static (compilation, _) =>
                    compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ModuleInitializerAttribute") is not null);

            var collected = roots.Collect().Combine(hasModuleInitializer);

            context.RegisterSourceOutput(collected, static (spc, input) => Execute(spc, input.Left, input.Right));
        }

        private static void Execute(
            SourceProductionContext context,
            ImmutableArray<RootCapture> captures,
            bool hasModuleInitializer)
        {
            var distinct = new Dictionary<string, INamedTypeSymbol>();
            var constructionRequests = new List<ConstructionRequest>();
            var seenRequests = new HashSet<string>();

            foreach (var capture in captures)
            {
                if (capture.Symbol is null)
                {
                    continue;
                }

                ReportRootDiagnostics(context, capture);

                var typeName = capture.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                distinct[typeName] = capture.Symbol;

                if (capture.ConstructNamespace is not null)
                {
                    var key = capture.ConstructNamespace + "|" + typeName;

                    if (seenRequests.Add(key))
                    {
                        constructionRequests.Add(new ConstructionRequest(typeName, capture.ConstructNamespace));
                    }
                }
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

            context.AddSource(
                "ModelBuilderGenerated.g.cs",
                SourceEmitter.Emit(models, constructionRequests, hasModuleInitializer));
        }

        private static void ReportRootDiagnostics(SourceProductionContext context, RootCapture capture)
        {
            var symbol = capture.Symbol!;
            var location = capture.Location ?? Location.None;

            var isUnbuildable = symbol.TypeKind == TypeKind.Interface
                                || symbol.IsAbstract
                                || ((symbol.TypeKind == TypeKind.Class || symbol.TypeKind == TypeKind.Struct)
                                    && BuildGraphWalker.HasAccessibleConstructor(symbol) == false);

            if (isUnbuildable == false)
            {
                return;
            }

            if (capture.IsTypeOfRoot)
            {
                // Model.Create(typeof(X)) names an unbuildable constant type (s12.7).
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.UnbuildableTypeOfRoot,
                        location,
                        symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

                return;
            }

            if (symbol.TypeKind == TypeKind.Interface || symbol.IsAbstract)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.UnmappedAbstractRoot,
                        location,
                        symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

                return;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.NoAccessibleConstructor,
                    location,
                    symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
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

            if (containingType != ModelTypeName && containingType != ConfigurationTypeName)
            {
                return default;
            }

            if (method.Name == "Mapping")
            {
                // Model.Mapping<TSource, TTarget>() makes the concrete TTarget a build root.
                if (method.TypeArguments.Length != 2)
                {
                    return default;
                }

                return new RootCapture(method.TypeArguments[1] as INamedTypeSymbol, invocation.GetLocation());
            }

            if (method.Name == "Construct")
            {
                // Model.Construct<T>() makes T a build root and requests typed From overloads in the
                // namespace of this call site.
                if (method.TypeArguments.Length != 1)
                {
                    return default;
                }

                var enclosing = context.SemanticModel.GetEnclosingSymbol(invocation.SpanStart, token);
                var ns = enclosing?.ContainingNamespace;
                var namespaceName = ns is null || ns.IsGlobalNamespace ? string.Empty : ns.ToDisplayString();

                return new RootCapture(
                    method.TypeArguments[0] as INamedTypeSymbol,
                    invocation.GetLocation(),
                    constructNamespace: namespaceName);
            }

            if (method.Name != "Create" && method.Name != "Populate" && method.Name != "Ignoring")
            {
                return default;
            }

            if (method.TypeArguments.Length != 1)
            {
                // Non-generic Model.Create(typeof(X)) - the typeof constant names a build root (s6.2.1).
                if (method.Name == "Create"
                    && containingType == ModelTypeName
                    && TryGetTypeOfArgument(invocation, context.SemanticModel, token, out var constantType))
                {
                    return new RootCapture(constantType, invocation.GetLocation(), isTypeOfRoot: true);
                }

                return default;
            }

            return new RootCapture(method.TypeArguments[0] as INamedTypeSymbol, invocation.GetLocation());
        }

        private static bool TryGetTypeOfArgument(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken token,
            out INamedTypeSymbol? type)
        {
            type = null;

            var firstArgument = invocation.ArgumentList.Arguments.Count > 0
                ? invocation.ArgumentList.Arguments[0].Expression
                : null;

            if (firstArgument is not TypeOfExpressionSyntax typeOf)
            {
                return false;
            }

            type = semanticModel.GetTypeInfo(typeOf.Type, token).Type as INamedTypeSymbol;

            return type is not null;
        }

        private static bool IsCandidateInvocation(SyntaxNode node)
        {
            return node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Name.Identifier.ValueText: "Create" or "Populate" or "Mapping" or "Ignoring" or "Construct"
                }
            };
        }

        private readonly struct RootCapture
        {
            public RootCapture(INamedTypeSymbol? symbol, Location location, bool isTypeOfRoot = false, string? constructNamespace = null)
            {
                Symbol = symbol;
                Location = location;
                IsTypeOfRoot = isTypeOfRoot;
                ConstructNamespace = constructNamespace;
            }

            public string? ConstructNamespace { get; }

            public bool IsTypeOfRoot { get; }

            public Location? Location { get; }

            public INamedTypeSymbol? Symbol { get; }
        }
    }
}
