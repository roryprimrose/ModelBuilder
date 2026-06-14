namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IValueSource{T}" /> interface
    ///     defines a strongly-typed source of values for a target type, replacing the separate value
    ///     generator and type creator abstractions of the previous design.
    /// </summary>
    /// <typeparam name="T">The type of value the source produces.</typeparam>
    public interface IValueSource<out T>
    {
        /// <summary>
        ///     Creates a value for the supplied target.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="target">The target the value is being built for.</param>
        /// <returns>The created value.</returns>
        T Create(IBuildContext context, in BuildTarget target);
    }
}
