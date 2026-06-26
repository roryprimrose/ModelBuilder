namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="GenerationModel" /> struct
    ///     aggregates everything the generator emits for a compilation: the builders for buildable types
    ///     and the value sources for enums.
    /// </summary>
    internal readonly struct GenerationModel : IEquatable<GenerationModel>
    {
        public GenerationModel(
            EquatableArray<BuildableModel> builders,
            EquatableArray<EnumModel> enums,
            EquatableArray<string> nullableUnderlyingTypes,
            EquatableArray<CollectionModel> collections)
        {
            Builders = builders;
            Enums = enums;
            NullableUnderlyingTypes = nullableUnderlyingTypes;
            Collections = collections;
        }

        public bool Equals(GenerationModel other)
        {
            return Builders.Equals(other.Builders)
                   && Enums.Equals(other.Enums)
                   && NullableUnderlyingTypes.Equals(other.NullableUnderlyingTypes)
                   && Collections.Equals(other.Collections);
        }

        public override bool Equals(object? obj)
        {
            return obj is GenerationModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Builders.GetHashCode();

                hash = hash * 397 ^ Enums.GetHashCode();
                hash = hash * 397 ^ NullableUnderlyingTypes.GetHashCode();
                hash = hash * 397 ^ Collections.GetHashCode();

                return hash;
            }
        }

        public bool IsEmpty =>
            Builders.Count == 0
            && Enums.Count == 0
            && NullableUnderlyingTypes.Count == 0
            && Collections.Count == 0;

        public EquatableArray<BuildableModel> Builders { get; }

        public EquatableArray<CollectionModel> Collections { get; }

        public EquatableArray<EnumModel> Enums { get; }

        public EquatableArray<string> NullableUnderlyingTypes { get; }
    }
}
