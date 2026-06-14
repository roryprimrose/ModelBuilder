namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IBuildContext" /> interface
    ///     is the per-build surface shared with generated builders and custom value sources. It exposes
    ///     the random source, build log, configuration, sibling-scope access, and the recursive
    ///     <see cref="Build{T}" /> entry point, without leaking the engine's internal build chain,
    ///     depth guards, or registries.
    /// </summary>
    public interface IBuildContext
    {
        /// <summary>
        ///     Gets the build configuration for the current build.
        /// </summary>
        IBuildConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the build log for the current build.
        /// </summary>
        IBuildLog Log { get; }

        /// <summary>
        ///     Gets the random source for the current build.
        /// </summary>
        IRandomSource Random { get; }

        /// <summary>
        ///     Builds a value for a member, resolving a registered value source first and then a
        ///     registered builder, with circular-reference and depth guards applied for built types.
        /// </summary>
        /// <typeparam name="T">The member type to build.</typeparam>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>The built value, or <c>default</c> when no source or builder is registered or a guard fired.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" /> or <paramref name="memberName" /> parameter is <c>null</c>.
        /// </exception>
        T Build<T>(Type declaringType, string memberName);

        /// <summary>
        ///     Opens a sibling scope for the instance currently being populated, so that members already
        ///     set on it can be read by value sources for later members via <see cref="GetSibling{T}" />.
        /// </summary>
        /// <returns>A token that closes the sibling scope when disposed.</returns>
        IDisposable EnterSiblingScope();

        /// <summary>
        ///     Reads a sibling member value already populated on the instance currently being built.
        /// </summary>
        /// <typeparam name="T">The expected sibling value type.</typeparam>
        /// <param name="memberName">The name of the sibling member.</param>
        /// <returns>
        ///     The sibling value, or <c>default</c> when there is no sibling scope or no matching member
        ///     has been recorded.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberName" /> parameter is <c>null</c>.</exception>
        T? GetSibling<T>(string memberName);

        /// <summary>
        ///     Returns a random item count for a collection within the configured minimum and maximum,
        ///     coercing the bounds so an out-of-order or negative configuration cannot throw.
        /// </summary>
        /// <returns>A count between the effective minimum and maximum, inclusive.</returns>
        int NextCount();

        /// <summary>
        ///     Records a member value into the current sibling scope so later members can read it.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="value">The value that was set on the member.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="memberName" /> parameter is <c>null</c>.</exception>
        void RecordSibling(string memberName, object? value);

        /// <summary>
        ///     Determines whether a member should be populated according to the configured ignore rules.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="memberType">The type of the member.</param>
        /// <returns><c>true</c> if the member should be populated; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" />, <paramref name="memberName" />, or
        ///     <paramref name="memberType" /> parameter is <c>null</c>.
        /// </exception>
        bool ShouldPopulate(Type declaringType, string memberName, Type memberType);
    }
}
