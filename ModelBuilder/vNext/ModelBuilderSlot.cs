namespace ModelBuilder.vNext
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="ModelBuilderSlot{T}" /> class
    ///     provides the typed-static slot that holds the builder for a closed type, giving the generated
    ///     generic build path a direct dispatch with no dictionary lookup.
    /// </summary>
    /// <typeparam name="T">The type the builder creates.</typeparam>
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "The per-closed-type static slot is the intended dispatch design (design.md s8.2.9).")]
    public static class ModelBuilderSlot<T>
    {
        /// <summary>
        ///     Gets or sets the builder registered for <typeparamref name="T" />.
        /// </summary>
        /// <returns>The registered builder, or <c>null</c> when none is registered.</returns>
        public static IModelBuilder<T>? Instance { get; set; }
    }
}
