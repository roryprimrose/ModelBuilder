namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IValueSource" /> interface
    ///     defines a non-generic source of values for an open set of target types. It is the escape
    ///     hatch for the rare case where a single source must handle types it cannot enumerate at
    ///     registration time, and it boxes value-type results.
    /// </summary>
    public interface IValueSource
    {
        /// <summary>
        ///     Creates a value for the supplied target.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="target">The target the value is being built for.</param>
        /// <returns>The created value, which may be <c>null</c>.</returns>
        object? Create(IBuildContext context, in BuildTarget target);
    }
}
