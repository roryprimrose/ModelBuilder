namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildTarget" /> struct
    ///     is used to describe what a value source is being asked to build: a target type and the
    ///     optional name of the member the value is destined for.
    /// </summary>
    public readonly record struct BuildTarget
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
