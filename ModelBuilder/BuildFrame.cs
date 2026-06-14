namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildFrame" /> struct
    ///     is used to describe a single step in the build path from the root type down to a member
    ///     currently being built.
    /// </summary>
    public readonly struct BuildFrame : IEquatable<BuildFrame>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildFrame" /> struct.
        /// </summary>
        /// <param name="declaringType">The type that declares the member being built.</param>
        /// <param name="memberName">The name of the member being built, or <c>null</c> for the root.</param>
        /// <param name="memberType">The type of the value being built at this frame.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" /> or <paramref name="memberType" /> parameter is <c>null</c>.
        /// </exception>
        public BuildFrame(Type declaringType, string? memberName, Type memberType)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            MemberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
            MemberName = memberName;
        }

        /// <inheritdoc />
        public bool Equals(BuildFrame other)
        {
            return DeclaringType == other.DeclaringType
                   && MemberType == other.MemberType
                   && string.Equals(MemberName, other.MemberName, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is BuildFrame other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = DeclaringType.GetHashCode();

                hash = hash * 397 ^ MemberType.GetHashCode();
                hash = hash * 397 ^ (MemberName is null ? 0 : StringComparer.Ordinal.GetHashCode(MemberName));

                return hash;
            }
        }

        /// <summary>
        ///     Determines whether two <see cref="BuildFrame" /> values are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BuildFrame left, BuildFrame right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two <see cref="BuildFrame" /> values are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BuildFrame left, BuildFrame right)
        {
            return left.Equals(right) == false;
        }

        /// <summary>
        ///     Gets the type that declares the member being built.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        ///     Gets the name of the member being built.
        /// </summary>
        /// <returns>The member name, or <c>null</c> when this frame is the root of the build path.</returns>
        public string? MemberName { get; }

        /// <summary>
        ///     Gets the type of the value being built at this frame.
        /// </summary>
        public Type MemberType { get; }
    }
}
