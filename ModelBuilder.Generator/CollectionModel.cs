namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="CollectionModel" /> struct
    ///     is the value-equatable description of a closed collection type the generator emits a value
    ///     source for.
    /// </summary>
    internal readonly struct CollectionModel : IEquatable<CollectionModel>
    {
        public CollectionModel(
            CollectionKind kind,
            string slotType,
            string sourceName,
            string elementType,
            string valueType,
            bool keyCanBeNull)
        {
            Kind = kind;
            SlotType = slotType;
            SourceName = sourceName;
            ElementType = elementType;
            ValueType = valueType;
            KeyCanBeNull = keyCanBeNull;
        }

        public bool Equals(CollectionModel other)
        {
            return Kind == other.Kind
                   && string.Equals(SlotType, other.SlotType, StringComparison.Ordinal)
                   && string.Equals(SourceName, other.SourceName, StringComparison.Ordinal)
                   && string.Equals(ElementType, other.ElementType, StringComparison.Ordinal)
                   && string.Equals(ValueType, other.ValueType, StringComparison.Ordinal)
                   && KeyCanBeNull == other.KeyCanBeNull;
        }

        public override bool Equals(object? obj)
        {
            return obj is CollectionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)Kind;

                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(SlotType);
                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(SourceName);
                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(ElementType);
                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(ValueType);
                hash = hash * 397 ^ KeyCanBeNull.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Gets the element type for arrays, lists and sets, or the key type for dictionaries.
        /// </summary>
        public string ElementType { get; }

        /// <summary>
        ///     Gets a value indicating whether the dictionary key type can be <c>null</c>.
        /// </summary>
        public bool KeyCanBeNull { get; }

        public CollectionKind Kind { get; }

        public string SlotType { get; }

        public string SourceName { get; }

        /// <summary>
        ///     Gets the value type for dictionaries; empty for other kinds.
        /// </summary>
        public string ValueType { get; }
    }
}
