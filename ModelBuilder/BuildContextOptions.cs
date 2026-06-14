namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="BuildContextOptions" /> class
    ///     is used to configure the behaviour of a <see cref="BuildContext" />.
    /// </summary>
    internal sealed class BuildContextOptions
    {
        /// <summary>
        ///     The default maximum build depth.
        /// </summary>
        public const int DefaultMaxDepth = 50;

        /// <summary>
        ///     The default percentage chance that a nullable value is produced as <c>null</c>. Kept low
        ///     so a populated value is the strong default, while still exercising the <c>null</c> path
        ///     often enough across a test run to surface null-handling bugs. Set
        ///     <see cref="NullPercentage" /> to 0 to never produce <c>null</c>.
        /// </summary>
        public const int DefaultNullPercentage = 5;

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
