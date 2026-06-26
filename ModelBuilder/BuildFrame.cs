namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildFrame" /> struct
    ///     is used to describe a single step in the build path from the root type down to a member
    ///     currently being built.
    /// </summary>
    public readonly record struct BuildFrame
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
