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
        ///     set on it can be read by value sources for later members via <see cref="GetSibling{T}(string)" />.
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
        ///     Reads the first recorded sibling member value among the supplied candidate names, so a
        ///     value source can resolve a sibling that may be spelled under any of several aliases (for
        ///     example <c>FirstName</c> or <c>GivenName</c>).
        /// </summary>
        /// <typeparam name="T">The expected sibling value type.</typeparam>
        /// <param name="memberNames">The candidate member names, tried in order.</param>
        /// <returns>
        ///     The first non-<c>default</c> sibling value matched by one of the supplied names, or
        ///     <c>default</c> when there is no sibling scope or none of the names has a recorded value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberNames" /> parameter is <c>null</c>.</exception>
        T? GetSibling<T>(params string[] memberNames);

        /// <summary>
        ///     Gets a value shared across the members of the instance currently being built, creating and
        ///     caching it on first request so that sibling value sources can derive related fields from a
        ///     single shared data item.
        /// </summary>
        /// <typeparam name="T">The type of the shared value.</typeparam>
        /// <param name="key">The key that identifies the shared value within the current build scope.</param>
        /// <param name="factory">The factory invoked to create the value when it has not yet been cached.</param>
        /// <returns>
        ///     The cached value when one already exists for <paramref name="key" />; otherwise the value
        ///     produced by <paramref name="factory" />, which is then cached for the remainder of the
        ///     scope. When no sibling scope is open the factory result is returned without caching.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="key" /> or <paramref name="factory" /> parameter is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     The shared value lives for the lifetime of the instance currently being populated; a
        ///     separate instance built elsewhere in the graph receives its own scope and therefore its own
        ///     shared value. This is the extension point for related entity-style data: several value
        ///     sources can agree on one cached data item (such as a single address row) by reading and
        ///     writing the same key, so the fields they produce stay internally consistent. A cached value
        ///     that is not assignable to <typeparamref name="T" /> is replaced by a new factory result.
        /// </remarks>
        T GetOrAddScopedValue<T>(string key, Func<IBuildContext, T> factory);

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
