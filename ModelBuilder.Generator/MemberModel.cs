namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="MemberModel" /> struct
    ///     describes a settable member to populate or a constructor parameter to supply.
    /// </summary>
    internal readonly struct MemberModel : IEquatable<MemberModel>
    {
        public MemberModel(string name, string typeName, string? defaultLiteral = null)
        {
            Name = name;
            TypeName = typeName;
            DefaultLiteral = defaultLiteral;
        }

        public bool Equals(MemberModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                   && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
                   && string.Equals(DefaultLiteral, other.DefaultLiteral, StringComparison.Ordinal);
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

                return hash;
            }
        }

        public string? DefaultLiteral { get; }

        public string Name { get; }

        public string TypeName { get; }
    }
}
