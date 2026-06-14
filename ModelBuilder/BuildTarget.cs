namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildTarget" /> struct
    ///     is used to describe what a value source is being asked to build: a target type and the
    ///     optional name of the member the value is destined for.
    /// </summary>
    public readonly struct BuildTarget : IEquatable<BuildTarget>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildTarget" /> struct.
        /// </summary>
        /// <param name="type">The type of the value to build.</param>
        /// <param name="memberName">The optional name of the member the value is being built for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public BuildTarget(Type type, string? memberName = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            MemberName = memberName;
        }

        /// <inheritdoc />
        public bool Equals(BuildTarget other)
        {
            return Type == other.Type
                   && string.Equals(MemberName, other.MemberName, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is BuildTarget other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Type.GetHashCode();

                hash = hash * 397 ^ (MemberName is null ? 0 : StringComparer.Ordinal.GetHashCode(MemberName));

                return hash;
            }
        }

        /// <summary>
        ///     Determines whether two <see cref="BuildTarget" /> values are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BuildTarget left, BuildTarget right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two <see cref="BuildTarget" /> values are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BuildTarget left, BuildTarget right)
        {
            return left.Equals(right) == false;
        }

        /// <summary>
        ///     Gets the optional name of the member the value is being built for.
        /// </summary>
        /// <returns>The member name, or <c>null</c> when the value is not being built for a named member.</returns>
        public string? MemberName { get; }

        /// <summary>
        ///     Gets the type of the value to build.
        /// </summary>
        public Type Type { get; }
    }
}
