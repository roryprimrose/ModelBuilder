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
        public static ImmutableArray<BuildableModel> Walk(IEnumerable<INamedTypeSymbol> roots)
        {
            var discovered = new Dictionary<string, INamedTypeSymbol>();
            var queue = new Queue<INamedTypeSymbol>();
            var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

            foreach (var root in roots)
            {
                Enqueue(root, queue, seen);
            }

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();

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
                    EnqueueIfNamed(parameter.Type, queue, seen);
                }

                foreach (var property in GetSettableProperties(type))
                {
                    EnqueueIfNamed(property.Type, queue, seen);
                }
            }

            var builderNames = new HashSet<string>();
            var models = ImmutableArray.CreateBuilder<BuildableModel>();

            foreach (var pair in discovered.OrderBy(p => p.Key, System.StringComparer.Ordinal))
            {
                models.Add(CreateModel(pair.Value, pair.Key, builderNames));
            }

            return models.ToImmutable();
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
                CreateBuilderName(fullyQualifiedName, builderNames),
                new EquatableArray<MemberModel>(ctorParameters.ToImmutable()),
                new EquatableArray<MemberModel>(members.ToImmutable()));
        }

        private static string CreateBuilderName(string fullyQualifiedName, HashSet<string> builderNames)
        {
            var chars = fullyQualifiedName.ToCharArray();

            for (var index = 0; index < chars.Length; index++)
            {
                if (char.IsLetterOrDigit(chars[index]) == false)
                {
                    chars[index] = '_';
                }
            }

            var candidate = new string(chars).Trim('_') + "Builder";

            var name = candidate;
            var suffix = 1;

            while (builderNames.Add(name) == false)
            {
                name = candidate + suffix;
                suffix++;
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

        private static void EnqueueIfNamed(ITypeSymbol type, Queue<INamedTypeSymbol> queue, HashSet<INamedTypeSymbol> seen)
        {
            if (type is INamedTypeSymbol named)
            {
                Enqueue(named, queue, seen);
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
            if (type.TypeKind != TypeKind.Class)
            {
                return false;
            }

            if (type.IsAbstract || type.IsStatic || type.IsGenericType)
            {
                return false;
            }

            if (type.SpecialType == SpecialType.System_String || type.SpecialType == SpecialType.System_Object)
            {
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
