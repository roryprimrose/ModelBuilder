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
                .Where(static symbol => symbol is not null)
                .Select(static (symbol, _) => symbol!);

            var collected = roots.Collect();

            context.RegisterSourceOutput(collected, static (spc, symbols) => Execute(spc, symbols));
        }

        private static void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> roots)
        {
            var distinct = roots
                .Where(static symbol => symbol is not null)
                .ToImmutableArray();

            if (distinct.Length == 0)
            {
                return;
            }

            var models = BuildGraphWalker.Walk(distinct);

            if (models.IsEmpty)
            {
                return;
            }

            context.AddSource("ModelBuilderGenerated.g.cs", SourceEmitter.Emit(models));
        }

        private static INamedTypeSymbol? GetRootType(GeneratorSyntaxContext context, CancellationToken token)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Expression is not MemberAccessExpressionSyntax)
            {
                return null;
            }

            if (context.SemanticModel.GetSymbolInfo(invocation, token).Symbol is not IMethodSymbol method)
            {
                return null;
            }

            var containingType = method.ContainingType?.ToDisplayString();

            if (containingType != ModelTypeName)
            {
                return null;
            }

            if (method.Name != "Create" && method.Name != "Populate")
            {
                return null;
            }

            if (method.TypeArguments.Length != 1)
            {
                return null;
            }

            return method.TypeArguments[0] as INamedTypeSymbol;
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
    }
}
