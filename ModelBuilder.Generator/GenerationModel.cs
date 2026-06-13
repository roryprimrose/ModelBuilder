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
        public GenerationModel(EquatableArray<BuildableModel> builders, EquatableArray<EnumModel> enums)
        {
            Builders = builders;
            Enums = enums;
        }

        public bool Equals(GenerationModel other)
        {
            return Builders.Equals(other.Builders) && Enums.Equals(other.Enums);
        }

        public override bool Equals(object? obj)
        {
            return obj is GenerationModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Builders.GetHashCode() * 397 ^ Enums.GetHashCode();
            }
        }

        public bool IsEmpty => Builders.Count == 0 && Enums.Count == 0;

        public EquatableArray<BuildableModel> Builders { get; }

        public EquatableArray<EnumModel> Enums { get; }
    }
}
