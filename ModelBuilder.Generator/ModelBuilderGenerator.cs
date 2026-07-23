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
        private const string GenerateModelBuilderAttributeName = "ModelBuilder.GenerateModelBuilderAttribute";

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var invocationRoots = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => IsCandidateInvocation(node),
                    static (ctx, token) => GetRootType(ctx, token))
                .Where(static capture => capture.Symbol is not null || capture.IsOpenMappingDeclaration)
                .Select(static (capture, _) => capture);

            var attributeRoots = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateModelBuilderAttributeName,
                    static (node, _) => IsCandidateAttributeTarget(node),
                    static (ctx, token) => GetAttributeRoots(ctx, token))
                .SelectMany(static (captures, _) => captures)
                .Where(static capture => capture.Symbol is not null);

            var roots = invocationRoots.Collect()
                .Combine(attributeRoots.Collect())
                .Select(static (pair, _) => pair.Left.AddRange(pair.Right));

            var hasModuleInitializer = context.CompilationProvider.Select(
                static (compilation, _) =>
                {
                    var symbol = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ModuleInitializerAttribute");

                    // A referenced assembly may declare its own internal ModuleInitializerAttribute
                    // polyfill (for example BenchmarkDotNet). GetTypeByMetadataName finds such a type
                    // even though the generated [ModuleInitializer] cannot bind to it, which fails with
                    // CS0122. Only treat the attribute as present when it is actually accessible from the
                    // compilation; otherwise the generator must emit its own accessible polyfill.
                    return symbol is not null
                        && compilation.IsSymbolAccessibleWithin(symbol, compilation.Assembly);
                });

            var collected = roots.Combine(hasModuleInitializer);

            context.RegisterSourceOutput(collected, static (spc, input) => Execute(spc, input.Left, input.Right));
        }

        private static void Execute(
            SourceProductionContext context,
            ImmutableArray<RootCapture> captures,
            bool hasModuleInitializer)
        {
            var distinct = new Dictionary<string, INamedTypeSymbol>();
            var constructionTypeNames = new List<string>();
            var seenRequests = new HashSet<string>();
            var closedMappingSourceDefinitions = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var openMappingDeclarations = new List<RootCapture>();

            foreach (var capture in captures)
            {
                if (capture.IsOpenMappingDeclaration)
                {
                    openMappingDeclarations.Add(capture);

                    continue;
                }

                if (capture.Symbol is null)
                {
                    continue;
                }

                ReportRootDiagnostics(context, capture);

                if (capture.MappingSourceDefinition is not null)
                {
                    closedMappingSourceDefinitions.Add(capture.MappingSourceDefinition);
                }

                var typeName = capture.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                distinct[typeName] = capture.Symbol;

                if (capture.IsConstructRoot && seenRequests.Add(typeName))
                {
                    constructionTypeNames.Add(typeName);
                }
            }

            foreach (var declaration in openMappingDeclarations)
            {
                ReportOpenMappingDiagnostics(context, declaration, closedMappingSourceDefinitions);
            }

            if (distinct.Count == 0)
            {
                return;
            }

            var models = BuildGraphWalker.Walk(
                distinct.Values,
                closedMappingSourceDefinitions,
                out var unsupportedCollectionShapes,
                out var unmappedMemberTypes);

            foreach (var unsupportedShape in unsupportedCollectionShapes)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.UnsupportedCollectionShape, Location.None, unsupportedShape));
            }

            foreach (var unmappedMemberType in unmappedMemberTypes)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.UnmappedAbstractMember, Location.None, unmappedMemberType));
            }

            if (models.IsEmpty)
            {
                return;
            }

            context.AddSource(
                "ModelBuilderGenerated.g.cs",
                SourceEmitter.Emit(models, constructionTypeNames, hasModuleInitializer));
        }

        private static void ReportOpenMappingDiagnostics(
            SourceProductionContext context,
            RootCapture declaration,
            HashSet<INamedTypeSymbol> closedMappingSourceDefinitions)
        {
            var source = declaration.OpenMappingSource!;
            var target = declaration.OpenMappingTarget!;
            var location = declaration.Location ?? Location.None;

            if (BuildGraphWalker.HasAccessibleConstructor(target) == false)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.OpenMappingTargetNoAccessibleConstructor,
                        location,
                        target.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                        source.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }

            if (closedMappingSourceDefinitions.Contains(source) == false)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.OpenMappingNeverUsedInClosedForm,
                        location,
                        source.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }
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
                if (method.TypeArguments.Length == 2)
                {
                    // Model.Mapping<TSource, TTarget>() makes the concrete TTarget a build root.
                    var closedSourceDefinition = (method.TypeArguments[0] as INamedTypeSymbol)?.OriginalDefinition;

                    return new RootCapture(
                        method.TypeArguments[1] as INamedTypeSymbol,
                        invocation.GetLocation(),
                        mappingSourceDefinition: closedSourceDefinition);
                }

                if (method.TypeArguments.Length == 0
                    && TryGetTypeOfArgument(invocation, context.SemanticModel, token, 0, out var mappingSourceType)
                    && TryGetTypeOfArgument(invocation, context.SemanticModel, token, 1, out var mappingTargetType))
                {
                    // Model.Mapping(typeof(TSource), typeof(TTarget)) - the Type-based overload. Both
                    // arguments must agree on being open (an open generic mapping declaration, validated
                    // and cross-referenced against closed Mapping<,> usages rather than walked as a root)
                    // or both closed (behaves exactly like the generic Mapping<TSource, TTarget>() overload).
                    if (mappingSourceType!.IsUnboundGenericType && mappingTargetType!.IsUnboundGenericType)
                    {
                        // Normalize the unbound generic type symbols from typeof(X<>) to their original
                        // definitions - the same representation used for the closed side below - so
                        // constructor lookups and cross-referencing are accurate and symbol-comparable.
                        return new RootCapture(
                            mappingSourceType.OriginalDefinition,
                            mappingTargetType.OriginalDefinition,
                            invocation.GetLocation());
                    }

                    if (mappingSourceType.IsUnboundGenericType == false && mappingTargetType!.IsUnboundGenericType == false)
                    {
                        return new RootCapture(
                            mappingTargetType,
                            invocation.GetLocation(),
                            mappingSourceDefinition: mappingSourceType.OriginalDefinition);
                    }
                }

                return default;
            }

            if (method.Name == "Construct")
            {
                // Model.Construct<T>() makes T a build root and requests typed From overloads.
                if (method.TypeArguments.Length != 1)
                {
                    return default;
                }

                return new RootCapture(
                    method.TypeArguments[0] as INamedTypeSymbol,
                    invocation.GetLocation(),
                    isConstructRoot: true);
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
                    && TryGetTypeOfArgument(invocation, context.SemanticModel, token, 0, out var constantType))
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
            int argumentIndex,
            out INamedTypeSymbol? type)
        {
            type = null;

            if (invocation.ArgumentList.Arguments.Count <= argumentIndex)
            {
                return false;
            }

            if (invocation.ArgumentList.Arguments[argumentIndex].Expression is not TypeOfExpressionSyntax typeOf)
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

        private static bool IsCandidateAttributeTarget(SyntaxNode node)
        {
            // Type-level [GenerateModelBuilder] is applied directly to a class/struct declaration.
            // Assembly-level [assembly: GenerateModelBuilder(typeof(X))] attaches to the
            // CompilationUnitSyntax of whichever file declares it; only files with a top-level
            // attribute list can possibly carry one, so filtering on AttributeLists.Count avoids
            // matching every file in the compilation.
            return node is ClassDeclarationSyntax or StructDeclarationSyntax
                || (node is CompilationUnitSyntax unit && unit.AttributeLists.Count > 0);
        }

        private static ImmutableArray<RootCapture> GetAttributeRoots(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            if (context.TargetSymbol is INamedTypeSymbol typeSymbol)
            {
                // [GenerateModelBuilder] applied directly to a type names that type as a root.
                var location = context.Attributes.Length > 0
                    ? context.Attributes[0].ApplicationSyntaxReference?.GetSyntax(token).GetLocation()
                    : null;

                return ImmutableArray.Create(new RootCapture(typeSymbol, location ?? Location.None));
            }

            if (context.TargetSymbol is IAssemblySymbol)
            {
                var builder = ImmutableArray.CreateBuilder<RootCapture>();

                foreach (var attribute in context.Attributes)
                {
                    // [assembly: GenerateModelBuilder(typeof(X))] names X as a root via the
                    // constructor's typeof(...) constant.
                    if (attribute.ConstructorArguments.Length == 0
                        || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol targetType)
                    {
                        continue;
                    }

                    var location = attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation() ?? Location.None;

                    builder.Add(new RootCapture(targetType, location));
                }

                return builder.ToImmutable();
            }

            return ImmutableArray<RootCapture>.Empty;
        }

        private readonly struct RootCapture
        {
            public RootCapture(
                INamedTypeSymbol? symbol,
                Location location,
                bool isTypeOfRoot = false,
                bool isConstructRoot = false,
                INamedTypeSymbol? mappingSourceDefinition = null)
            {
                Symbol = symbol;
                Location = location;
                IsTypeOfRoot = isTypeOfRoot;
                IsConstructRoot = isConstructRoot;
                MappingSourceDefinition = mappingSourceDefinition;
                IsOpenMappingDeclaration = false;
                OpenMappingSource = null;
                OpenMappingTarget = null;
            }

            public RootCapture(INamedTypeSymbol openSource, INamedTypeSymbol openTarget, Location location)
            {
                Symbol = null;
                Location = location;
                IsTypeOfRoot = false;
                IsConstructRoot = false;
                MappingSourceDefinition = null;
                IsOpenMappingDeclaration = true;
                OpenMappingSource = openSource;
                OpenMappingTarget = openTarget;
            }

            public bool IsConstructRoot { get; }

            public bool IsOpenMappingDeclaration { get; }

            public bool IsTypeOfRoot { get; }

            public Location? Location { get; }

            public INamedTypeSymbol? MappingSourceDefinition { get; }

            public INamedTypeSymbol? OpenMappingSource { get; }

            public INamedTypeSymbol? OpenMappingTarget { get; }

            public INamedTypeSymbol? Symbol { get; }
        }
    }
}
