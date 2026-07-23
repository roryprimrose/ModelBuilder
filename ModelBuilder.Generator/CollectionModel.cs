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
            bool keyCanBeNull,
            bool retryOnKeyCollision = false,
            bool isCustomType = false)
        {
            Kind = kind;
            SlotType = slotType;
            SourceName = sourceName;
            ElementType = elementType;
            ValueType = valueType;
            KeyCanBeNull = keyCanBeNull;
            RetryOnKeyCollision = retryOnKeyCollision;
            IsCustomType = isCustomType;
        }

        public bool Equals(CollectionModel other)
        {
            return Kind == other.Kind
                   && string.Equals(SlotType, other.SlotType, StringComparison.Ordinal)
                   && string.Equals(SourceName, other.SourceName, StringComparison.Ordinal)
                   && string.Equals(ElementType, other.ElementType, StringComparison.Ordinal)
                   && string.Equals(ValueType, other.ValueType, StringComparison.Ordinal)
                   && KeyCanBeNull == other.KeyCanBeNull
                   && RetryOnKeyCollision == other.RetryOnKeyCollision
                   && IsCustomType == other.IsCustomType;
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
                hash = hash * 397 ^ RetryOnKeyCollision.GetHashCode();
                hash = hash * 397 ^ IsCustomType.GetHashCode();

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

        /// <summary>
        ///     Gets a value indicating whether a colliding random key is retried (up to a bounded
        ///     attempt count) rather than silently overwriting the earlier entry.
        /// </summary>
        public bool RetryOnKeyCollision { get; }

        public CollectionKind Kind { get; }

        public string SlotType { get; }

        public string SourceName { get; }

        /// <summary>
        ///     Gets the value type for dictionaries; empty for other kinds.
        /// </summary>
        public string ValueType { get; }

        /// <summary>
        ///     Gets a value indicating whether <see cref="SlotType" /> is a user-defined type that was
        ///     classified by inheriting from, or directly implementing, one of the well-known
        ///     collection shapes rather than being one of those shapes itself. When <c>true</c>, the
        ///     generated source constructs <see cref="SlotType" /> directly (via its own parameterless
        ///     constructor) instead of a well-known BCL backing type.
        /// </summary>
        public bool IsCustomType { get; }
    }
}
