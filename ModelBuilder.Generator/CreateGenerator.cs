namespace ModelBuilder.Generator;

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class ModelCreateIncrementalGenerator : IIncrementalGenerator
{
    private readonly string _version;

    public ModelCreateIncrementalGenerator()
    {
        var assembly = GetType().Assembly;
        _version = assembly.GetName().Version.ToString() ?? "1.0";
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register a syntax provider to find invocations of Model.Create<T>()
        var methodCalls = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsModelCreateInvocation(node),
                transform: static (context, _) => GetTypeToGenerate(context))
            .Where(x => x != null);

        var nodeSet = methodCalls.Collect();

        context.RegisterSourceOutput(nodeSet, Execute);
    }

    private static string CreateSafeName(string name, string defaultName = "")
    {
        if (string.IsNullOrWhiteSpace(name)
            && string.IsNullOrWhiteSpace(defaultName) == false)
        {
            // Use the default value if one has been supplied
            name = defaultName;
        }

        return name.Replace('.', '_').Trim();
    }

    // Extract the type argument from Model.Create<T>()
    private static BindTarget? GetTypeToGenerate(GeneratorSyntaxContext context)
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
                && typeSymbol.ContainingNamespace.ToDisplayString() == "ModelBuilder")
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

                var callingTypeNamespace = declaredSymbol
                    .ContainingNamespace.ToDisplayString();

                var genericType = context.SemanticModel.GetTypeInfo(typeArgument).Type;

                if (genericType == null)
                {
                    return null;
                }

                var targetNamespace = genericType.ContainingNamespace.ToDisplayString();
                var targetClassName = genericType.Name;

                return new(callingTypeNamespace, targetNamespace, targetClassName);
            }
        }

        return null;
    }

    // Check if the syntax node is a potential Model.Create<T>() call
    private static bool IsModelCreateInvocation(SyntaxNode node)
    {
        //return node is InvocationExpressionSyntax invocation &&
        //       invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
        //       memberAccess.Name is GenericNameSyntax genericName &&
        //       genericName.Identifier.Text == "Create";

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

    private void WriteCreateProxy(SourceProductionContext context, string callingNamespace,
        ICollection<TargetType?> targetTypes)
    {
        var callerNamespace = callingNamespace;
        var hasNamespace = string.IsNullOrWhiteSpace(callerNamespace) == false;
        
        // TODO: Make this namespace relative to the calling project
        var builderNamespace = typeof(ModelCreateIncrementalGenerator).Namespace! + ".SourceGenerator_Builders";

        var builder = new StringBuilder();
        var padding = string.Empty;

        if (hasNamespace)
        {
            padding = "    ";
            builder.AppendLine($"namespace {callerNamespace}");
            builder.AppendLine("{");
        }

        builder.AppendLine(
            $"{padding}[System.CodeDom.Compiler.GeneratedCode(\"{typeof(ModelCreateIncrementalGenerator)}\", \"{_version}\")]");
        builder.AppendLine($"{padding}public static partial class Model");
        builder.AppendLine($"{padding}{{");

        // Write the proxy method that will handle the transition from T to target type
        builder.AppendLine($"{padding}    public static T Create<T>()");
        builder.AppendLine($"{padding}    {{");

        foreach (var targetType in targetTypes)
        {
            if (targetType == null)
            {
                continue;
            }

            builder.AppendLine($"{padding}        if (typeof(T) == typeof({targetType.FullTypeName}))");
            builder.AppendLine($"{padding}        {{");
            builder.AppendLine($"{padding}            // Custom initialization logic for {targetType.FullTypeName}");
            builder.AppendLine($"{padding}            return (T)(object)({builderNamespace}.Builder.Create_{targetType.SafeTypeName}());");
            builder.AppendLine($"{padding}        }}");
            builder.AppendLine();
        }

        builder.AppendLine($"{padding}        // The type requested was not included in this source generated code");
        builder.AppendLine(
            $"{padding}        throw new System.InvalidOperationException($\"The requested type {{typeof(T)}} does not have source generator code created for it\");");
        builder.AppendLine($"{padding}    }}");

        if (hasNamespace)
        {
            builder.AppendLine($"{padding}}}");
        }

        builder.AppendLine("}");

        var source = builder.ToString();

        var filename = CreateSafeName(callerNamespace, "Global");

        filename += "_CreateProxy.g.cs";

        context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
    }

    private void Execute(SourceProductionContext context, ImmutableArray<BindTarget?> bindTargets)
    {
        // Separate the bind targets into groups based on the calling namespace
        var callersByNamespace = bindTargets.GroupBy(x => x?.CallingNamespace);

        foreach (var caller in callersByNamespace)
        {
            if (caller.Key == null)
            {
                continue;
            }
            
            // Get the distinct target types for the calling namespace
            var targetTypes = caller.Select(BuildTargetType).Distinct().ToList();

            WriteCreateProxy(context, caller.Key, targetTypes);
        }

        // Get the unique bind targets regardless of which calling namespace calls for a Create<T>()
        // TODO: This will need to expand the list of target types to include all the other types they reference
        var uniqueTargetTypes = bindTargets.Select(BuildTargetType).Distinct().ToList();

        WriteTypeBuilders(context, uniqueTargetTypes);
    }

    private void WriteTypeBuilders(SourceProductionContext context, List<TargetType?> targetTypes)
    {
        var builder = new StringBuilder();

        // TODO: Make this namespace relative to the calling project
        var builderNamespace = typeof(ModelCreateIncrementalGenerator).Namespace! + ".SourceGenerator_Builders";

        var padding = "    ";
        builder.AppendLine($"namespace {builderNamespace}");
        builder.AppendLine("{");

        builder.AppendLine(
            $"{padding}[System.CodeDom.Compiler.GeneratedCode(\"{typeof(ModelCreateIncrementalGenerator)}\", \"{_version}\")]");
        builder.AppendLine($"{padding}public static class Builder");
        builder.AppendLine($"{padding}{{");

        foreach (var targetType in targetTypes)
        {
            if (targetType == null)
            {
                continue;
            }

            // Write the proxy method that will handle the transition from T to target type
            builder.AppendLine($"{padding}    public static {targetType.FullTypeName} Create_{targetType.SafeTypeName}()");
            builder.AppendLine($"{padding}    {{");
            builder.AppendLine($"{padding}        return new {targetType.FullTypeName}();");
            builder.AppendLine($"{padding}    }}");
            builder.AppendLine();
        }

        builder.AppendLine($"{padding}}}");

        builder.AppendLine("}");

        var source = builder.ToString();

        var filename = CreateSafeName(builderNamespace + ".Builder.g.cs");

        context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
    }

    private static TargetType? BuildTargetType(BindTarget? x)
    {
        if (x == null)
        {
            return null;
        }

        var fullTypeName = x.TargetClass;

        if (string.IsNullOrWhiteSpace(x.TargetNamespace) == false)
        {
            fullTypeName = x.TargetNamespace + "." + fullTypeName;
        }

        var safeTypeName = CreateSafeName(fullTypeName);

        return new TargetType(x.TargetNamespace, x.TargetClass, fullTypeName, safeTypeName);
    }

    private record BindTarget(string CallingNamespace, string TargetNamespace, string TargetClass);

    private record TargetType(string TargetNamespace, string TargetClass, string FullTypeName, string SafeTypeName);
}