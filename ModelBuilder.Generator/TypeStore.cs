namespace ModelBuilder.Generator
{
    using Microsoft.CodeAnalysis;

    internal class TypeStore : ITypeStore
    {
        private static readonly Dictionary<TargetType, TargetTypeDefinition> _targetTypeCache = new();

        public TargetType AddType(ITypeSymbol typeSymbol, bool includeConstructors = false)
        {
            var targetNamespace = typeSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
            var targetClassName = typeSymbol.Name;
            var targetType = new TargetType(targetNamespace, targetClassName);

            Stack<TargetType> typesFound = new();

            PopulateCache(typesFound, targetType, typeSymbol);

            return targetType;
        }

        public TargetTypeDefinition GetDefinition(TargetType targetType)
        {
            return _targetTypeCache[targetType];
        }

        public string CreateSafeName(string name, string defaultName = "")
        {
            if (string.IsNullOrWhiteSpace(name)
                && string.IsNullOrWhiteSpace(defaultName) == false)
            {
                // Use the default value if one has been supplied
                name = defaultName;
            }

            return name.Replace('.', '_').Trim();
        }

        private void PopulateCache(Stack<TargetType> typesFound, TargetType targetType, ITypeSymbol typeSymbol)
        {
            // We need to track a stack of property types to break circular references
            if (typesFound.Contains(targetType))
            {
                // We have a circular reference
                return;
            }

            typesFound.Push(targetType);

            if (_targetTypeCache.ContainsKey(targetType) == false)
            {
                // We haven't seen this type yet
                // Build the full metadata for this type

                // Find the constructor with the least number of parameters
                var constructors = typeSymbol
                    .GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(m => m.MethodKind == MethodKind.Constructor)
                    .OrderBy(c => c.Parameters.Length)
                    .FirstOrDefault();

                List<TargetParameter> parameters = new();

                if (constructors?.Parameters != null)
                {
                    foreach (var constructorParameter in constructors.Parameters)
                    {
                        var parameterType = constructorParameter.Type;
                        var parameterTypeNamespace =
                            parameterType.ContainingNamespace?.ToDisplayString() ?? string.Empty;

                        var targetParameter = new TargetParameter(constructorParameter.Name,
                            parameterTypeNamespace,
                            parameterType.Name);

                        // Recurse into this type
                        PopulateCache(typesFound, targetParameter, parameterType);

                        parameters.Add(targetParameter);
                    }
                }

                // Calculate properties on target type
                List<TargetProperty> targetProperties = new();

                var declaredProperties = typeSymbol.GetMembers()
                    .OfType<IPropertySymbol>();

                foreach (var declaredProperty in declaredProperties)
                {
                    if (declaredProperty.IsIndexer)
                    {
                        // We ignore indexer properties
                        continue;
                    }

                    var propertyType = declaredProperty.Type;
                    var propertyTypeNamespace = propertyType.ContainingNamespace?.ToDisplayString() ?? string.Empty;

                    var targetProperty =
                        new TargetProperty(declaredProperty.Name, propertyTypeNamespace, propertyType.Name);

                    // Recurse into this type
                    PopulateCache(typesFound, targetProperty, propertyType);

                    targetProperties.Add(targetProperty);
                }

                var fullTypeName = targetType.TypeName;

                if (string.IsNullOrWhiteSpace(targetType.TypeNamespace) == false)
                {
                    fullTypeName = targetType.TypeNamespace + "." + fullTypeName;
                }

                var safeTypeName = CreateSafeName(fullTypeName);

                var definition = new TargetTypeDefinition(fullTypeName, safeTypeName, targetProperties, parameters);

                _targetTypeCache[targetType] = definition;
            }
        }
    }
}