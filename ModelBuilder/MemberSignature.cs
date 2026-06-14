namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="MemberSignature" /> struct
    ///     is used to describe a member to a type-agnostic ignore predicate, exposing the information
    ///     available to the generator: the declaring type, the member name, and the member type.
    /// </summary>
    public readonly record struct MemberSignature
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
