namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="INullableBuilder" />
    ///     interface defines the members that control whether a <c>null</c> value can be built when a type is requested.
    /// </summary>
    public interface INullableBuilder
    {
        /// <summary>
        ///     Gets or sets whether this type can return a <c>null</c> value.
        /// </summary>
        bool AllowNull { get; set; }

        /// <summary>
        ///     Gets or sets the percentage change that a null could be return when <see cref="AllowNull" /> is <c>true</c>.
        /// </summary>
        int NullPercentageChance { get; set; }
    }
}