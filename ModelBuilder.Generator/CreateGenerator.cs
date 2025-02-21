namespace ModelBuilder.Generator;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class ModelCreateIncrementalGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor _descriptor = new DiagnosticDescriptor(
        id: "MB0001",
        title: "Source Generator Exception",
        messageFormat: "An exception occurred in the source generator: {0}",
        category: "SourceGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (Debugger.IsAttached == false)
        //{
        //    Debugger.Launch();
        //}

        var typeStore = new TypeStore();

        // Register a syntax provider to find invocations of Model.Create<T>()
        var methodCalls = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsModelCreateInvocation(node),
                transform: (syntaxContext, _) => GetTypeToGenerate(syntaxContext, typeStore))
            .Where(x => x != null);

        var nodeSet = methodCalls.Collect();

        context.RegisterSourceOutput(nodeSet,
            (productionContext, bindTargets) => Execute(productionContext, bindTargets, typeStore));
    }

    private static BindTarget? GetTypeToGenerate(GeneratorSyntaxContext context, ITypeStore typeStore)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Name is GenericNameSyntax genericName
            && genericName.TypeArgumentList.Arguments.Count == 1)
        {
            var typeArgument = genericName.TypeArgumentList.Arguments[0];
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess.Expression).Symbol;

            // Ensure the method is part of Model in the ModelBuilder namespace
            if (symbol is ITypeSymbol typeSymbol
                && typeSymbol.Name == "Model"
                && (typeSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty) == "ModelBuilder")
            {
                var typeDeclarationSyntax = context.Node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();

                if (typeDeclarationSyntax == null)
                {
                    return null;
                }

                var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax);

                if (declaredSymbol == null)
                {
                    return null;
                }

                var genericType = context.SemanticModel.GetTypeInfo(typeArgument).Type;

                if (genericType == null)
                {
                    return null;
                }

                var callingTypeNamespace = declaredSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

                var targetType = typeStore.AddType(genericType);

                var bindTarget = new BindTarget(callingTypeNamespace, targetType);

                return bindTarget;
            }
        }

        return null;
    }

    private static bool IsModelCreateInvocation(SyntaxNode node)
    {
        if (node.Language != "C#")
        {
            return false;
        }

        var methodSyntax = node as InvocationExpressionSyntax;

        if (methodSyntax == null)
        {
            return false;
        }

        var memberAccessSyntax = methodSyntax.Expression as MemberAccessExpressionSyntax;

        if (memberAccessSyntax == null)
        {
            return false;
        }

        // method.Identifier.ValueText 
        var simpleNameSyntax = memberAccessSyntax.Name as GenericNameSyntax;

        if (simpleNameSyntax == null)
        {
            return false;
        }

        if (simpleNameSyntax.Identifier.Text != "Create")
        {
            return false;
        }

        //if (simpleNameSyntax.TypeArgumentList.Arguments.Count != 1)
        //{
        //    return false;
        //}

        //var declaringClass = methodSyntax.Parent as ClassDeclarationSyntax; // Store the reference to the declaring class

        //if (declaringClass == null)
        //{
        //    return false;
        //}

        //if (declaringClass.Identifier.ValueText != "Model")
        //{
        //    return false;
        //}

        return true;
    }

    private static Diagnostic ReportException(Exception ex)
    {
        var failureDetails =
            $"Exception Message: {ex.Message}\nStack Trace: {ex.StackTrace}\nSource: {ex.Source}\nTarget Site: {ex.TargetSite}";

        return Diagnostic.Create(_descriptor, Location.None, failureDetails);
    }

    private void Execute(SourceProductionContext context, ImmutableArray<BindTarget?> bindTargets, ITypeStore typeStore)
    {
        try
        {
            var builder = new SourceBuilder(typeStore);

            builder.Build(context, bindTargets);
        }
        catch (Exception ex)
        {
            var diagnostic = ReportException(ex);

            context.ReportDiagnostic(diagnostic);
        }
    }
}