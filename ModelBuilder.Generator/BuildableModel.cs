namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="BuildableModel" /> struct
    ///     is the value-equatable description of a type the generator emits a builder for.
    /// </summary>
    internal readonly struct BuildableModel : IEquatable<BuildableModel>
    {
        public BuildableModel(
            string fullyQualifiedName,
            string builderName,
            EquatableArray<MemberModel> constructorParameters,
            EquatableArray<MemberModel> members)
        {
            FullyQualifiedName = fullyQualifiedName;
            BuilderName = builderName;
            ConstructorParameters = constructorParameters;
            Members = members;
        }

        public bool Equals(BuildableModel other)
        {
            return string.Equals(FullyQualifiedName, other.FullyQualifiedName, StringComparison.Ordinal)
                   && string.Equals(BuilderName, other.BuilderName, StringComparison.Ordinal)
                   && ConstructorParameters.Equals(other.ConstructorParameters)
                   && Members.Equals(other.Members);
        }

        public override bool Equals(object? obj)
        {
            return obj is BuildableModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(FullyQualifiedName);

                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(BuilderName);
                hash = hash * 397 ^ ConstructorParameters.GetHashCode();
                hash = hash * 397 ^ Members.GetHashCode();

                return hash;
            }
        }

        public string BuilderName { get; }

        public EquatableArray<MemberModel> ConstructorParameters { get; }

        public string FullyQualifiedName { get; }

        public EquatableArray<MemberModel> Members { get; }
    }
}
