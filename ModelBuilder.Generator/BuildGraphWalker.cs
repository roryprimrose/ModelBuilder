namespace ModelBuilder.Generator
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    ///     The <see cref="BuildGraphWalker" /> class
    ///     walks outward from a set of root types and produces the value-equatable models for every
    ///     concrete, accessible type reachable through constructor parameters and settable members.
    /// </summary>
    internal static class BuildGraphWalker
    {
        /// <summary>
        ///     Determines whether the type has a public constructor the generated code can call.
        /// </summary>
        public static bool HasAccessibleConstructor(INamedTypeSymbol type)
        {
            return SelectConstructor(type) != null;
        }

        public static GenerationModel Walk(IEnumerable<INamedTypeSymbol> roots)
        {
            var discovered = new Dictionary<string, INamedTypeSymbol>();
            var enums = new Dictionary<string, INamedTypeSymbol>();
            var nullables = new SortedSet<string>(System.StringComparer.Ordinal);
            var collections = new Dictionary<string, ITypeSymbol>();
            var queue = new Queue<INamedTypeSymbol>();
            var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

            foreach (var root in roots)
            {
                Enqueue(root, queue, seen);
            }

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();

                if (type.TypeKind == TypeKind.Enum)
                {
                    enums[type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = type;

                    continue;
                }

                if (IsBuildable(type) == false)
                {
                    continue;
                }

                var name = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (discovered.ContainsKey(name))
                {
                    continue;
                }

                discovered.Add(name, type);

                foreach (var parameter in SelectConstructor(type)?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty)
                {
                    Visit(parameter.Type, queue, seen, nullables, collections);
                }

                foreach (var property in GetSettableProperties(type))
                {
                    Visit(property.Type, queue, seen, nullables, collections);
                }
            }

            var builderNames = new HashSet<string>();
            var sourceNames = new HashSet<string>();
            var builders = ImmutableArray.CreateBuilder<BuildableModel>();

            foreach (var pair in discovered.OrderBy(p => p.Key, System.StringComparer.Ordinal))
            {
                builders.Add(CreateModel(pair.Value, pair.Key, builderNames));
            }

            var enumModels = ImmutableArray.CreateBuilder<EnumModel>();

            foreach (var pair in enums.OrderBy(p => p.Key, System.StringComparer.Ordinal))
            {
                enumModels.Add(CreateEnumModel(pair.Value, pair.Key, sourceNames));
            }

            var collectionModels = ImmutableArray.CreateBuilder<CollectionModel>();

            foreach (var pair in collections.OrderBy(p => p.Key, System.StringComparer.Ordinal))
            {
                collectionModels.Add(CreateCollectionModel(pair.Value, pair.Key, sourceNames));
            }

            return new GenerationModel(
                new EquatableArray<BuildableModel>(builders.ToImmutable()),
                new EquatableArray<EnumModel>(enumModels.ToImmutable()),
                new EquatableArray<string>(nullables.ToImmutableArray()),
                new EquatableArray<CollectionModel>(collectionModels.ToImmutable()));
        }

        private static CollectionModel CreateCollectionModel(ITypeSymbol type, string slotType, HashSet<string> sourceNames)
        {
            TryClassifyCollection(type, out var kind, out var element, out var value);

            return new CollectionModel(
                kind,
                slotType,
                CreateName(slotType, "ValueSource", sourceNames),
                element?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty,
                value?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty,
                kind == CollectionKind.Dictionary && element?.IsReferenceType == true);
        }

        private static EnumModel CreateEnumModel(INamedTypeSymbol type, string fullyQualifiedName, HashSet<string> sourceNames)
        {
            var isFlags = type.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");

            var members = ImmutableArray.CreateBuilder<string>();

            foreach (var member in type.GetMembers())
            {
                if (member is not IFieldSymbol field || field.IsConst == false || field.ConstantValue is null)
                {
                    continue;
                }

                if (isFlags && IsZero(field.ConstantValue))
                {
                    // The zero member contributes nothing to a flags combination.
                    continue;
                }

                members.Add(field.Name);
            }

            return new EnumModel(
                fullyQualifiedName,
                CreateName(fullyQualifiedName, "ValueSource", sourceNames),
                new EquatableArray<string>(members.ToImmutable()),
                isFlags);
        }

        private static bool IsZero(object constantValue)
        {
            return constantValue switch
            {
                byte b => b == 0,
                sbyte b => b == 0,
                short s => s == 0,
                ushort s => s == 0,
                int i => i == 0,
                uint i => i == 0,
                long l => l == 0,
                ulong l => l == 0,
                _ => false
            };
        }

        private static void Visit(
            ITypeSymbol type,
            Queue<INamedTypeSymbol> queue,
            HashSet<INamedTypeSymbol> seen,
            SortedSet<string> nullables,
            Dictionary<string, ITypeSymbol> collections)
        {
            if (type is IArrayTypeSymbol array)
            {
                if (array.Rank == 1)
                {
                    collections[type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = type;

                    Visit(array.ElementType, queue, seen, nullables, collections);
                }

                return;
            }

            if (type is not INamedTypeSymbol named)
            {
                return;
            }

            if (named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                && named.TypeArguments.Length == 1)
            {
                var underlying = named.TypeArguments[0];

                nullables.Add(underlying.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

                Visit(underlying, queue, seen, nullables, collections);

                return;
            }

            if (TryClassifyCollection(named, out _, out var element, out var value))
            {
                collections[named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = named;

                if (element != null)
                {
                    Visit(element, queue, seen, nullables, collections);
                }

                if (value != null)
                {
                    Visit(value, queue, seen, nullables, collections);
                }

                return;
            }

            Enqueue(named, queue, seen);
        }

        private static bool TryClassifyCollection(
            ITypeSymbol type,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value)
        {
            kind = CollectionKind.Array;
            element = null;
            value = null;

            if (type is IArrayTypeSymbol array)
            {
                if (array.Rank != 1)
                {
                    return false;
                }

                kind = CollectionKind.Array;
                element = array.ElementType;

                return true;
            }

            if (type is not INamedTypeSymbol { IsGenericType: true } named)
            {
                return false;
            }

            var definition = named.OriginalDefinition.ToDisplayString();
            var args = named.TypeArguments;

            switch (definition)
            {
                case "System.Collections.Generic.List<T>":
                case "System.Collections.Generic.IList<T>":
                case "System.Collections.Generic.ICollection<T>":
                case "System.Collections.Generic.IEnumerable<T>":
                case "System.Collections.Generic.IReadOnlyList<T>":
                case "System.Collections.Generic.IReadOnlyCollection<T>":
                    kind = CollectionKind.List;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.HashSet<T>":
                case "System.Collections.Generic.ISet<T>":
                    kind = CollectionKind.Set;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Dictionary<TKey, TValue>":
                case "System.Collections.Generic.IDictionary<TKey, TValue>":
                case "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>":
                    kind = CollectionKind.Dictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                default:
                    return false;
            }
        }

        private static BuildableModel CreateModel(
            INamedTypeSymbol type,
            string fullyQualifiedName,
            HashSet<string> builderNames)
        {
            var constructor = SelectConstructor(type);
            var ctorParameters = ImmutableArray.CreateBuilder<MemberModel>();

            if (constructor != null)
            {
                foreach (var parameter in constructor.Parameters)
                {
                    ctorParameters.Add(
                        new MemberModel(
                            parameter.Name,
                            parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                }
            }

            var members = ImmutableArray.CreateBuilder<MemberModel>();

            foreach (var property in GetSettableProperties(type))
            {
                members.Add(
                    new MemberModel(
                        property.Name,
                        property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
            }

            return new BuildableModel(
                fullyQualifiedName,
                CreateName(fullyQualifiedName, "Builder", builderNames),
                new EquatableArray<MemberModel>(ctorParameters.ToImmutable()),
                new EquatableArray<MemberModel>(members.ToImmutable()));
        }

        private static string CreateName(string fullyQualifiedName, string suffix, HashSet<string> usedNames)
        {
            var chars = fullyQualifiedName.ToCharArray();

            for (var index = 0; index < chars.Length; index++)
            {
                if (char.IsLetterOrDigit(chars[index]) == false)
                {
                    chars[index] = '_';
                }
            }

            var candidate = new string(chars).Trim('_') + suffix;

            var name = candidate;
            var attempt = 1;

            while (usedNames.Add(name) == false)
            {
                name = candidate + attempt;
                attempt++;
            }

            return name;
        }

        private static void Enqueue(INamedTypeSymbol type, Queue<INamedTypeSymbol> queue, HashSet<INamedTypeSymbol> seen)
        {
            if (seen.Add(type))
            {
                queue.Enqueue(type);
            }
        }

        private static IEnumerable<IPropertySymbol> GetSettableProperties(INamedTypeSymbol type)
        {
            var seenNames = new HashSet<string>();

            for (var current = type; current != null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    if (member is not IPropertySymbol property)
                    {
                        continue;
                    }

                    if (property.IsStatic
                        || property.IsIndexer
                        || property.DeclaredAccessibility != Accessibility.Public)
                    {
                        continue;
                    }

                    if (property.SetMethod is not { } setMethod
                        || setMethod.DeclaredAccessibility != Accessibility.Public
                        || setMethod.IsInitOnly)
                    {
                        continue;
                    }

                    if (seenNames.Add(property.Name))
                    {
                        yield return property;
                    }
                }
            }
        }

        private static bool IsAccessible(INamedTypeSymbol type)
        {
            for (var current = type; current != null; current = current.ContainingType)
            {
                if (current.DeclaredAccessibility != Accessibility.Public
                    && current.DeclaredAccessibility != Accessibility.Internal
                    && current.DeclaredAccessibility != Accessibility.NotApplicable)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsBuildable(INamedTypeSymbol type)
        {
            if (type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct)
            {
                return false;
            }

            if (type.IsAbstract || type.IsStatic || type.IsGenericType)
            {
                return false;
            }

            if (type.IsTupleType)
            {
                return false;
            }

            if (type.SpecialType != SpecialType.None)
            {
                // Primitives, string, decimal, IntPtr, etc. are leaf value types, not buildable graphs.
                return false;
            }

            var ns = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;

            if (ns == "System" || ns.StartsWith("System.", System.StringComparison.Ordinal))
            {
                return false;
            }

            if (IsAccessible(type) == false)
            {
                return false;
            }

            return SelectConstructor(type) != null;
        }

        private static IMethodSymbol? SelectConstructor(INamedTypeSymbol type)
        {
            IMethodSymbol? candidate = null;

            foreach (var constructor in type.InstanceConstructors)
            {
                if (constructor.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }

                if (constructor.Parameters.Length == 0)
                {
                    return constructor;
                }

                if (candidate == null || constructor.Parameters.Length < candidate.Parameters.Length)
                {
                    candidate = constructor;
                }
            }

            return candidate;
        }
    }
}
