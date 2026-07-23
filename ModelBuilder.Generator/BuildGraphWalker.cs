namespace ModelBuilder.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Text;
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
            return Walk(roots, out _);
        }

        public static GenerationModel Walk(IEnumerable<INamedTypeSymbol> roots, out ImmutableArray<string> unsupportedCollectionShapes)
        {
            var discovered = new Dictionary<string, INamedTypeSymbol>();
            var enums = new Dictionary<string, INamedTypeSymbol>();
            var nullables = new SortedSet<string>(System.StringComparer.Ordinal);
            var collections = new Dictionary<string, ITypeSymbol>();
            var unsupported = new HashSet<string>();
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

                if (TryClassifyCollection(type, out _, out var rootElement, out var rootValue, unsupported))
                {
                    collections[type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = type;

                    if (rootElement != null)
                    {
                        Visit(rootElement, queue, seen, nullables, collections, unsupported);
                    }

                    if (rootValue != null)
                    {
                        Visit(rootValue, queue, seen, nullables, collections, unsupported);
                    }

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
                    Visit(parameter.Type, queue, seen, nullables, collections, unsupported);
                }

                foreach (var property in GetSettableProperties(type))
                {
                    Visit(property.Type, queue, seen, nullables, collections, unsupported);
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

            unsupportedCollectionShapes = unsupported.OrderBy(name => name, System.StringComparer.Ordinal).ToImmutableArray();

            return new GenerationModel(
                new EquatableArray<BuildableModel>(builders.ToImmutable()),
                new EquatableArray<EnumModel>(enumModels.ToImmutable()),
                new EquatableArray<string>(nullables.ToImmutableArray()),
                new EquatableArray<CollectionModel>(collectionModels.ToImmutable()));
        }

        private static readonly HashSet<CollectionKind> _keyedCollectionKinds = new HashSet<CollectionKind>
        {
            CollectionKind.Dictionary,
            CollectionKind.SortedDictionary,
            CollectionKind.SortedList,
            CollectionKind.ConcurrentDictionary,
            CollectionKind.ReadOnlyDictionary,
            CollectionKind.ImmutableDictionary,
            CollectionKind.ImmutableSortedDictionary
        };

        private static readonly HashSet<CollectionKind> _retryOnKeyCollisionKinds = new HashSet<CollectionKind>
        {
            CollectionKind.SortedDictionary,
            CollectionKind.SortedList,
            CollectionKind.ConcurrentDictionary
        };

        private static CollectionModel CreateCollectionModel(ITypeSymbol type, string slotType, HashSet<string> sourceNames)
        {
            TryClassifyCollection(type, out var kind, out var element, out var value, null);

            var keyCanBeNull = _keyedCollectionKinds.Contains(kind) && element?.IsReferenceType == true;
            var retryOnKeyCollision = _retryOnKeyCollisionKinds.Contains(kind);
            var isCustomType = IsCustomCollectionType(type);

            return new CollectionModel(
                kind,
                slotType,
                CreateName(slotType, "ValueSource", sourceNames),
                element?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty,
                value?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty,
                keyCanBeNull,
                retryOnKeyCollision,
                isCustomType);
        }

        private static bool IsCustomCollectionType(ITypeSymbol type)
        {
            if (type is not INamedTypeSymbol named)
            {
                // Arrays are always built as the well-known T[] shape.
                return false;
            }

            // A type is "custom" when it reached collection classification without directly
            // matching one of the well-known BCL definitions/interfaces itself - i.e. it was
            // classified through inheritance or interface implementation instead.
            return TryClassifyByDefinition(named.OriginalDefinition.ToDisplayString(), named.TypeArguments, out _, out _, out _) == false;
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
            Dictionary<string, ITypeSymbol> collections,
            HashSet<string> unsupported)
        {
            if (type is IArrayTypeSymbol array)
            {
                if (array.Rank == 1)
                {
                    collections[type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = type;

                    Visit(array.ElementType, queue, seen, nullables, collections, unsupported);
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

                Visit(underlying, queue, seen, nullables, collections, unsupported);

                return;
            }

            if (TryClassifyCollection(named, out _, out var element, out var value, unsupported))
            {
                collections[named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = named;

                if (element != null)
                {
                    Visit(element, queue, seen, nullables, collections, unsupported);
                }

                if (value != null)
                {
                    Visit(value, queue, seen, nullables, collections, unsupported);
                }

                return;
            }

            if (named.IsGenericType && IsUnsupportedCollectionShape(named))
            {
                return;
            }

            Enqueue(named, queue, seen);
        }


        private static readonly string[] _unsupportedCollectionShapeDefinitions =
        {
            "System.ArraySegment<T>",
            "System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection",
            "System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection",
            "System.Collections.Generic.SortedDictionary<TKey, TValue>.KeyCollection",
            "System.Collections.Generic.SortedDictionary<TKey, TValue>.ValueCollection"
        };

        private static bool IsUnsupportedCollectionShape(INamedTypeSymbol named)
        {
            var definition = named.OriginalDefinition.ToDisplayString();

            return Array.IndexOf(_unsupportedCollectionShapeDefinitions, definition) >= 0;
        }

        private static bool TryClassifyCollection(
            ITypeSymbol type,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value,
            HashSet<string>? unsupportedShapes)
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

            if (type is not INamedTypeSymbol named)
            {
                return false;
            }

            if (IsUnsupportedCollectionShape(named))
            {
                unsupportedShapes?.Add(named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

                return false;
            }

            if (named.IsGenericType
                && TryClassifyByDefinition(named.OriginalDefinition.ToDisplayString(), named.TypeArguments, out kind, out element, out value))
            {
                return true;
            }

            // A user-defined type that derives from a known mutable BCL collection (for example
            // `class WidgetBag : Collection<Widget>`) or implements `ICollection<T>`/
            // `IDictionary<TKey, TValue>` directly (for example a hand-written `IList<T>`
            // implementation) is built the same way as its matched shape, but constructed as the
            // requested type itself so its own behavior/state is preserved.
            if (named.TypeKind == TypeKind.Class
                && named.IsAbstract == false
                && IsAccessible(named)
                && HasPublicParameterlessConstructor(named)
                && (TryClassifyByBaseDefinition(named, out kind, out element, out value)
                    || TryClassifyByInterfaceImplementation(named, out kind, out element, out value)))
            {
                return true;
            }

            return false;
        }

        private static bool TryClassifyByDefinition(
            string definition,
            ImmutableArray<ITypeSymbol> args,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value)
        {
            kind = CollectionKind.Array;
            element = null;
            value = null;

            switch (definition)
            {
                // Single-type-argument, "Add"-based kinds.
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
                case "System.Collections.Generic.IReadOnlySet<T>":
                    kind = CollectionKind.Set;
                    element = args[0];

                    return true;
                case "System.Collections.ObjectModel.Collection<T>":
                    kind = CollectionKind.Collection;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Queue<T>":
                    kind = CollectionKind.Queue;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Stack<T>":
                    kind = CollectionKind.Stack;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.SortedSet<T>":
                    kind = CollectionKind.SortedSet;
                    element = args[0];

                    return true;
                case "System.Collections.ObjectModel.ObservableCollection<T>":
                    kind = CollectionKind.ObservableCollection;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentBag<T>":
                    kind = CollectionKind.ConcurrentBag;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentQueue<T>":
                    kind = CollectionKind.ConcurrentQueue;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentStack<T>":
                    kind = CollectionKind.ConcurrentStack;
                    element = args[0];

                    return true;

                // Two-type-argument, keyed kinds.
                case "System.Collections.Generic.Dictionary<TKey, TValue>":
                case "System.Collections.Generic.IDictionary<TKey, TValue>":
                case "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>":
                    kind = CollectionKind.Dictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Generic.SortedDictionary<TKey, TValue>":
                    kind = CollectionKind.SortedDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Generic.SortedList<TKey, TValue>":
                    kind = CollectionKind.SortedList;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>":
                    kind = CollectionKind.ConcurrentDictionary;
                    element = args[0];
                    value = args[1];

                    return true;

                // Read-only wrapper kinds.
                case "System.Collections.ObjectModel.ReadOnlyCollection<T>":
                    kind = CollectionKind.ReadOnlyCollection;
                    element = args[0];

                    return true;
                case "System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>":
                    kind = CollectionKind.ReadOnlyDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.ObjectModel.ReadOnlyObservableCollection<T>":
                    kind = CollectionKind.ReadOnlyObservableCollection;
                    element = args[0];

                    return true;

                // Immutable, single-type-argument kinds.
                case "System.Collections.Immutable.ImmutableArray<T>":
                    kind = CollectionKind.ImmutableArray;
                    element = args[0];

                    return true;
                case "System.Collections.Immutable.ImmutableList<T>":
                case "System.Collections.Immutable.IImmutableList<T>":
                    kind = CollectionKind.ImmutableList;
                    element = args[0];

                    return true;
                case "System.Collections.Immutable.ImmutableHashSet<T>":
                case "System.Collections.Immutable.IImmutableSet<T>":
                    kind = CollectionKind.ImmutableHashSet;
                    element = args[0];

                    return true;
                case "System.Collections.Immutable.ImmutableSortedSet<T>":
                    kind = CollectionKind.ImmutableSortedSet;
                    element = args[0];

                    return true;
                case "System.Collections.Immutable.ImmutableQueue<T>":
                case "System.Collections.Immutable.IImmutableQueue<T>":
                    kind = CollectionKind.ImmutableQueue;
                    element = args[0];

                    return true;
                case "System.Collections.Immutable.ImmutableStack<T>":
                case "System.Collections.Immutable.IImmutableStack<T>":
                    kind = CollectionKind.ImmutableStack;
                    element = args[0];

                    return true;

                // Immutable, keyed kinds.
                case "System.Collections.Immutable.ImmutableDictionary<TKey, TValue>":
                case "System.Collections.Immutable.IImmutableDictionary<TKey, TValue>":
                    kind = CollectionKind.ImmutableDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Immutable.ImmutableSortedDictionary<TKey, TValue>":
                    kind = CollectionKind.ImmutableSortedDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                default:
                    return false;
            }
        }

        private static bool TryClassifyByBaseDefinition(
            INamedTypeSymbol named,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value)
        {
            for (var ancestor = named.BaseType;
                 ancestor != null && ancestor.SpecialType != SpecialType.System_Object;
                 ancestor = ancestor.BaseType)
            {
                if (ancestor.IsGenericType == false)
                {
                    continue;
                }

                if (TryClassifyByMutableBaseDefinition(ancestor.OriginalDefinition.ToDisplayString(), ancestor.TypeArguments, out kind, out element, out value))
                {
                    return true;
                }
            }

            kind = CollectionKind.Array;
            element = null;
            value = null;

            return false;
        }

        private static bool TryClassifyByMutableBaseDefinition(
            string definition,
            ImmutableArray<ITypeSymbol> args,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value)
        {
            // Only the concrete, non-sealed, parameterless-constructible BCL collection kinds are
            // classifiable as an inheritance base. Read-only wrappers and immutable kinds are
            // excluded here: they have no usable parameterless constructor (or, for the immutable
            // kinds, are sealed and cannot be subclassed at all).
            kind = CollectionKind.Array;
            element = null;
            value = null;

            switch (definition)
            {
                case "System.Collections.Generic.List<T>":
                    kind = CollectionKind.List;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.HashSet<T>":
                    kind = CollectionKind.Set;
                    element = args[0];

                    return true;
                case "System.Collections.ObjectModel.Collection<T>":
                    kind = CollectionKind.Collection;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Queue<T>":
                    kind = CollectionKind.Queue;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Stack<T>":
                    kind = CollectionKind.Stack;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.SortedSet<T>":
                    kind = CollectionKind.SortedSet;
                    element = args[0];

                    return true;
                case "System.Collections.ObjectModel.ObservableCollection<T>":
                    kind = CollectionKind.ObservableCollection;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentBag<T>":
                    kind = CollectionKind.ConcurrentBag;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentQueue<T>":
                    kind = CollectionKind.ConcurrentQueue;
                    element = args[0];

                    return true;
                case "System.Collections.Concurrent.ConcurrentStack<T>":
                    kind = CollectionKind.ConcurrentStack;
                    element = args[0];

                    return true;
                case "System.Collections.Generic.Dictionary<TKey, TValue>":
                    kind = CollectionKind.Dictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Generic.SortedDictionary<TKey, TValue>":
                    kind = CollectionKind.SortedDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Generic.SortedList<TKey, TValue>":
                    kind = CollectionKind.SortedList;
                    element = args[0];
                    value = args[1];

                    return true;
                case "System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>":
                    kind = CollectionKind.ConcurrentDictionary;
                    element = args[0];
                    value = args[1];

                    return true;
                default:
                    return false;
            }
        }

        private static bool TryClassifyByInterfaceImplementation(
            INamedTypeSymbol named,
            out CollectionKind kind,
            out ITypeSymbol? element,
            out ITypeSymbol? value)
        {
            // A custom type that implements IDictionary<TKey, TValue> directly (rather than
            // deriving from a known keyed base type) is keyed - checked first since
            // IDictionary<TKey, TValue> also implements ICollection<KeyValuePair<TKey, TValue>>.
            foreach (var candidateInterface in named.AllInterfaces)
            {
                if (candidateInterface.IsGenericType
                    && candidateInterface.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.IDictionary<TKey, TValue>")
                {
                    kind = CollectionKind.Dictionary;
                    element = candidateInterface.TypeArguments[0];
                    value = candidateInterface.TypeArguments[1];

                    return true;
                }
            }

            // A custom type that implements ICollection<T> directly (for example a hand-written
            // IList<T> implementation, since IList<T> extends ICollection<T>) is Add-based.
            foreach (var candidateInterface in named.AllInterfaces)
            {
                if (candidateInterface.IsGenericType
                    && candidateInterface.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.ICollection<T>")
                {
                    kind = CollectionKind.List;
                    element = candidateInterface.TypeArguments[0];
                    value = null;

                    return true;
                }
            }

            kind = CollectionKind.Array;
            element = null;
            value = null;

            return false;
        }

        private static bool HasPublicParameterlessConstructor(INamedTypeSymbol type)
        {
            foreach (var constructor in type.InstanceConstructors)
            {
                if (constructor.DeclaredAccessibility == Accessibility.Public && constructor.Parameters.Length == 0)
                {
                    return true;
                }
            }

            return false;
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
                            parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                            FormatDefaultLiteral(parameter)));
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

            var allConstructors = ImmutableArray.CreateBuilder<ConstructorModel>();

            foreach (var publicConstructor in type.InstanceConstructors)
            {
                if (publicConstructor.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }

                var parameters = ImmutableArray.CreateBuilder<MemberModel>();

                foreach (var parameter in publicConstructor.Parameters)
                {
                    parameters.Add(
                        new MemberModel(
                            parameter.Name,
                            parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                            FormatDefaultLiteral(parameter)));
                }

                allConstructors.Add(
                    new ConstructorModel(
                        new EquatableArray<MemberModel>(parameters.ToImmutable()),
                        new EquatableArray<string>(RenderConstructorDoc(publicConstructor))));
            }

            return new BuildableModel(
                fullyQualifiedName,
                CreateName(fullyQualifiedName, "Builder", builderNames),
                new EquatableArray<MemberModel>(ctorParameters.ToImmutable()),
                new EquatableArray<MemberModel>(members.ToImmutable()),
                new EquatableArray<ConstructorModel>(allConstructors.ToImmutable()));
        }

        private static ImmutableArray<string> RenderConstructorDoc(IMethodSymbol constructor)
        {
            var xml = constructor.GetDocumentationCommentXml();

            if (string.IsNullOrWhiteSpace(xml))
            {
                return ImmutableArray<string>.Empty;
            }

            System.Xml.Linq.XElement root;

            try
            {
                root = System.Xml.Linq.XElement.Parse(xml);
            }
            catch (System.Xml.XmlException)
            {
                return ImmutableArray<string>.Empty;
            }

            var lines = ImmutableArray.CreateBuilder<string>();

            var summary = root.Element("summary");

            if (summary != null)
            {
                var text = Normalize(summary.Value);

                if (text.Length > 0)
                {
                    lines.Add("/// <summary>");
                    lines.Add("/// " + Escape(text));
                    lines.Add("/// </summary>");
                }
            }

            foreach (var param in root.Elements("param"))
            {
                var name = param.Attribute("name")?.Value;

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                lines.Add("/// <param name=\"" + name + "\">" + Escape(Normalize(param.Value)) + "</param>");
            }

            return lines.ToImmutable();
        }

        private static string Normalize(string value)
        {
            return string.Join(" ", value.Split(new[] { '\r', '\n', '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        private static string Escape(string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
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

        private static string? FormatDefaultLiteral(IParameterSymbol parameter)
        {
            if (parameter.HasExplicitDefaultValue == false)
            {
                return null;
            }

            var value = parameter.ExplicitDefaultValue;
            var type = parameter.Type;

            if (value == null)
            {
                // A null explicit default on a non-nullable value type represents default(T); otherwise
                // it is a null literal.
                if (type.IsValueType
                    && (type is not INamedTypeSymbol named
                        || named.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T))
                {
                    var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    return "default(" + typeName + ")";
                }

                return "null";
            }

            if (type.TypeKind == TypeKind.Enum)
            {
                // The default carries the enum's underlying integral value; cast it back to the enum
                // type so the literal is valid even when the value has no named member.
                var enumName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var underlying = ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);

                return "(" + enumName + ")(" + underlying + ")";
            }

            var effectiveType = type;

            if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            {
                effectiveType = nullable.TypeArguments[0];
            }

            // Numeric literals need their type suffix so the literal is valid for the parameter type
            // (for example a float default must be written as 1.5F, not 1.5).
            switch (effectiveType.SpecialType)
            {
                case SpecialType.System_Single:
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture) + "F";
                case SpecialType.System_Double:
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture) + "D";
                case SpecialType.System_Decimal:
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + "M";
                case SpecialType.System_Int64:
                    return Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + "L";
                case SpecialType.System_UInt64:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + "UL";
                case SpecialType.System_UInt32:
                    return Convert.ToUInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + "U";
                default:
                    return FormatConstantLiteral(value);
            }
        }

        private static string FormatConstantLiteral(object value)
        {
            switch (value)
            {
                case bool flag:
                    return flag ? "true" : "false";
                case string text:
                    return "\"" + EscapeContent(text, false) + "\"";
                case char character:
                    return "'" + EscapeContent(character.ToString(), true) + "'";
                default:
                    // Remaining integral primitives (byte, sbyte, short, ushort, int).
                    return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);
            }
        }

        private static string EscapeContent(string value, bool inChar)
        {
            var builder = new StringBuilder(value.Length);

            foreach (var character in value)
            {
                switch (character)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\0':
                        builder.Append("\\0");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '"' when inChar == false:
                        builder.Append("\\\"");
                        break;
                    case '\'' when inChar:
                        builder.Append("\\'");
                        break;
                    default:
                        if (char.IsControl(character))
                        {
                            builder.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            builder.Append(character);
                        }

                        break;
                }
            }

            return builder.ToString();
        }
    }
}
