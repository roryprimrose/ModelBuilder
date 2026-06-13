namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="MemberModel" /> struct
    ///     describes a settable member to populate or a constructor parameter to supply.
    /// </summary>
    internal readonly struct MemberModel : IEquatable<MemberModel>
    {
        public MemberModel(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        public bool Equals(MemberModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                   && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is MemberModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return StringComparer.Ordinal.GetHashCode(Name) * 397
                       ^ StringComparer.Ordinal.GetHashCode(TypeName);
            }
        }

        public string Name { get; }

        public string TypeName { get; }
    }
}
