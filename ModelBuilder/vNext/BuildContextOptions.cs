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
        ///     The default percentage chance that a nullable value is produced as <c>null</c>.
        /// </summary>
        public const int DefaultNullPercentage = 10;

        /// <summary>
        ///     The default minimum number of items generated for a collection.
        /// </summary>
        public const int DefaultMinCount = 1;

        /// <summary>
        ///     The default maximum number of items generated for a collection.
        /// </summary>
        public const int DefaultMaxCount = 10;

        /// <summary>
        ///     Gets or sets the maximum depth the build may descend before it is considered to have
        ///     exceeded a safe limit.
        /// </summary>
        public int MaxDepth { get; set; } = DefaultMaxDepth;

        /// <summary>
        ///     Gets or sets the maximum number of items generated for a collection.
        /// </summary>
        public int MaxCount { get; set; } = DefaultMaxCount;

        /// <summary>
        ///     Gets or sets the minimum number of items generated for a collection.
        /// </summary>
        public int MinCount { get; set; } = DefaultMinCount;

        /// <summary>
        ///     Gets or sets the percentage chance (0 to 100) that a nullable value is produced as
        ///     <c>null</c> rather than a value.
        /// </summary>
        public int NullPercentage { get; set; } = DefaultNullPercentage;
    }
}
