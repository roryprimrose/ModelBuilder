namespace ModelBuilder.vNext
{
    /// <summary>
    ///     The <see cref="BuildContextOptions" /> class
    ///     is used to configure the behaviour of a <see cref="BuildContext" />.
    /// </summary>
    public sealed class BuildContextOptions
    {
        /// <summary>
        ///     The default maximum build depth.
        /// </summary>
        public const int DefaultMaxDepth = 50;

        /// <summary>
        ///     Gets or sets the maximum depth the build may descend before it is considered to have
        ///     exceeded a safe limit.
        /// </summary>
        public int MaxDepth { get; set; } = DefaultMaxDepth;
    }
}
