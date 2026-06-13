namespace ModelBuilder.vNext
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="ValueSource{T}" /> class
    ///     provides the typed-static slot that holds the value source for a closed type, giving the
    ///     generated generic build path a direct, boxing-free dispatch with no dictionary lookup.
    /// </summary>
    /// <typeparam name="T">The type the value source produces.</typeparam>
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "The per-closed-type static slot is the intended dispatch design (design.md s8.2.9).")]
    public static class ValueSource<T>
    {
        /// <summary>
        ///     Gets or sets the value source registered for <typeparamref name="T" />.
        /// </summary>
        /// <returns>The registered value source, or <c>null</c> when none is registered.</returns>
        public static IValueSource<T>? Instance { get; set; }
    }
}
