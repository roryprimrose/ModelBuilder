namespace ModelBuilder.CodeFixes
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ModelCreateArgsCodeFixProvider" /> class
    ///     rewrites a removed v8 <c>Model.Create&lt;T&gt;(args)</c> call into the v9
    ///     <c>Model.Construct&lt;T&gt;().From(args)</c> typed-construction form.
    /// </summary>
    /// <remarks>
    ///     The fix is offered on the obsolete-error diagnostic (<c>CS0619</c>) the
    ///     <c>Model.Create&lt;T&gt;(params object?[]?)</c> shim raises, and is only registered when the
    ///     call site resolves to that member, so unrelated obsolete usages are left untouched.
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModelCreateArgsCodeFixProvider))]
    [Shared]
    public sealed class ModelCreateArgsCodeFixProvider : CodeFixProvider
    {
        private const string ModelTypeName = "ModelBuilder.Model";

        private const string Title = "Use Model.Construct<T>().From(...)";

        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0619");

        /// <inheritdoc />
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc />
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return;
            }

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            if (semanticModel == null)
            {
                return;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                var invocation = node.FirstAncestorOrSelf<InvocationExpressionSyntax>();

                if (invocation == null)
                {
                    continue;
                }

                if (IsModelCreateWithArguments(invocation, semanticModel, context.CancellationToken) == false)
                {
                    continue;
                }

                var action = CodeAction.Create(
                    Title,
                    cancellationToken => RewriteAsync(context.Document, root, invocation, cancellationToken),
                    equivalenceKey: Title);

                context.RegisterCodeFix(action, diagnostic);
            }
        }

        private static bool IsModelCreateWithArguments(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
                || memberAccess.Name is not GenericNameSyntax genericName
                || genericName.Identifier.ValueText != "Create"
                || genericName.TypeArgumentList.Arguments.Count != 1)
            {
                return false;
            }

            var symbolInfo = semanticModel.GetSymbolInfo(invocation, cancellationToken);

            if (symbolInfo.Symbol is not IMethodSymbol method)
            {
                return false;
            }

            return method.Name == "Create"
                   && method.ContainingType?.ToDisplayString() == ModelTypeName;
        }

        private static Task<Document> RewriteAsync(
            Document document,
            SyntaxNode root,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
            var createName = (GenericNameSyntax)memberAccess.Name;
            var receiver = memberAccess.Expression;

            // receiver.Construct<T>()
            var constructName = SyntaxFactory.GenericName(SyntaxFactory.Identifier("Construct"))
                .WithTypeArgumentList(createName.TypeArgumentList);

            var constructAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                receiver,
                constructName);

            var constructInvocation = SyntaxFactory.InvocationExpression(constructAccess);

            // (receiver.Construct<T>()).From(args)
            var fromAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                constructInvocation,
                SyntaxFactory.IdentifierName("From"));

            var fromInvocation = SyntaxFactory.InvocationExpression(fromAccess, invocation.ArgumentList)
                .WithTriviaFrom(invocation);

            var newRoot = root.ReplaceNode(invocation, fromInvocation);

            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
