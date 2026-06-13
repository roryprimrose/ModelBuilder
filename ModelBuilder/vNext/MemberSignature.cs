namespace ModelBuilder.vNext
{
    using System;

    /// <summary>
    ///     The <see cref="MemberSignature" /> struct
    ///     is used to describe a member to a type-agnostic ignore predicate, exposing the information
    ///     available to the generator: the declaring type, the member name, and the member type.
    /// </summary>
    public readonly struct MemberSignature : IEquatable<MemberSignature>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberSignature" /> struct.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="memberType">The type of the member.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" />, <paramref name="name" />, or
        ///     <paramref name="memberType" /> parameter is <c>null</c>.
        /// </exception>
        public MemberSignature(Type declaringType, string name, Type memberType)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MemberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
        }

        /// <inheritdoc />
        public bool Equals(MemberSignature other)
        {
            return DeclaringType == other.DeclaringType
                   && MemberType == other.MemberType
                   && string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is MemberSignature other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = DeclaringType.GetHashCode();

                hash = hash * 397 ^ MemberType.GetHashCode();
                hash = hash * 397 ^ StringComparer.Ordinal.GetHashCode(Name);

                return hash;
            }
        }

        /// <summary>
        ///     Determines whether two <see cref="MemberSignature" /> values are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MemberSignature left, MemberSignature right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two <see cref="MemberSignature" /> values are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MemberSignature left, MemberSignature right)
        {
            return left.Equals(right) == false;
        }

        /// <summary>
        ///     Gets the type that declares the member.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        ///     Gets the type of the member.
        /// </summary>
        public Type MemberType { get; }

        /// <summary>
        ///     Gets the name of the member.
        /// </summary>
        public string Name { get; }
    }
}
