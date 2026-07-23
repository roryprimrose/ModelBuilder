namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="MemberModel" /> struct
    ///     describes a settable member to populate or a constructor parameter to supply.
    /// </summary>
    internal readonly struct MemberModel : IEquatable<MemberModel>
    {
        public MemberModel(string name, string typeName, string? defaultLiteral = null, string? runtimeTypeName = null)
        {
            Name = name;
            TypeName = typeName;
            DefaultLiteral = defaultLiteral;
            RuntimeTypeName = runtimeTypeName ?? typeName;
        }

        public bool Equals(MemberModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                   && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
                   && string.Equals(DefaultLiteral, other.DefaultLiteral, StringComparison.Ordinal)
                   && string.Equals(RuntimeTypeName, other.RuntimeTypeName, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is MemberModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(Name) * 397
                           ^ StringComparer.Ordinal.GetHashCode(TypeName);

                hash = hash * 397 ^ (DefaultLiteral == null ? 0 : StringComparer.Ordinal.GetHashCode(DefaultLiteral));
                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(RuntimeTypeName);

                return hash;
            }
        }

        public string? DefaultLiteral { get; }

        public string Name { get; }

        /// <summary>
        ///     Gets the type name to use inside a <c>typeof(...)</c> expression. This strips any
        ///     top-level nullable reference annotation from <see cref="TypeName" /> (which <c>typeof</c>
        ///     rejects with CS8639) while preserving nested annotations, for example
        ///     <c>List&lt;object?&gt;</c> stays as-is but <c>object?</c> becomes <c>object</c>.
        /// </summary>
        public string RuntimeTypeName { get; }

        public string TypeName { get; }
    }
}
