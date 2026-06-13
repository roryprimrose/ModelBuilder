namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="EnumModel" /> struct
    ///     is the value-equatable description of an enum the generator emits a value source for.
    /// </summary>
    internal readonly struct EnumModel : IEquatable<EnumModel>
    {
        public EnumModel(
            string fullyQualifiedName,
            string sourceName,
            EquatableArray<string> memberNames,
            bool isFlags)
        {
            FullyQualifiedName = fullyQualifiedName;
            SourceName = sourceName;
            MemberNames = memberNames;
            IsFlags = isFlags;
        }

        public bool Equals(EnumModel other)
        {
            return string.Equals(FullyQualifiedName, other.FullyQualifiedName, StringComparison.Ordinal)
                   && string.Equals(SourceName, other.SourceName, StringComparison.Ordinal)
                   && IsFlags == other.IsFlags
                   && MemberNames.Equals(other.MemberNames);
        }

        public override bool Equals(object? obj)
        {
            return obj is EnumModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(FullyQualifiedName);

                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(SourceName);
                hash = hash * 397 ^ IsFlags.GetHashCode();
                hash = hash * 397 ^ MemberNames.GetHashCode();

                return hash;
            }
        }

        public string FullyQualifiedName { get; }

        public bool IsFlags { get; }

        public EquatableArray<string> MemberNames { get; }

        public string SourceName { get; }
    }
}
