namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="BuildOptions" /> class
    ///     holds the tunable, build-wide settings that control collection sizes, the frequency of
    ///     <c>null</c> values and the maximum graph depth. Configure it through
    ///     <see cref="Model.SetOptions" />, <see cref="IModelConfiguration.SetOptions" /> or
    ///     <see cref="IBuildConfiguration.SetOptions" />.
    /// </summary>
    public sealed class BuildOptions
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
        /// <remarks>
        ///     A value below <see cref="MinCount" /> is coerced up to <see cref="MinCount" /> when a
        ///     collection length is drawn, so setting <see cref="MaxCount" /> without
        ///     <see cref="MinCount" /> cannot throw.
        /// </remarks>
        public int MaxCount { get; set; } = DefaultMaxCount;

        /// <summary>
        ///     Gets or sets the minimum number of items generated for a collection.
        /// </summary>
        /// <remarks>
        ///     A negative value is coerced up to zero when a collection length is drawn.
        /// </remarks>
        public int MinCount { get; set; } = DefaultMinCount;

        /// <summary>
        ///     Gets or sets the percentage chance (0 to 100) that a nullable value is produced as
        ///     <c>null</c> rather than a value.
        /// </summary>
        /// <returns>
        ///     A value of 0 never produces <c>null</c>; a value of 100 always produces <c>null</c>.
        /// </returns>
        public int NullPercentage { get; set; } = DefaultNullPercentage;

        /// <summary>
        ///     Gets or sets a value indicating whether the automatic build path uses an optional
        ///     constructor parameter's declared default value instead of generating one.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if optional constructor parameters use their declared default value on the
        ///     automatic build path; otherwise, <c>false</c> and a value is generated for them.
        /// </returns>
        /// <remarks>
        ///     This applies to the constructor selected by <see cref="Model.Create{T}()" /> and the
        ///     non-generic create path. The explicit <see cref="Model.Construct{T}" /> path always lets
        ///     the caller decide per argument, because the generated <c>From</c> overloads expose each
        ///     optional parameter's default.
        /// </remarks>
        public bool UseConstructorDefaults { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a settable member that already holds a non-default
        ///     value is retained instead of being overwritten with a generated value.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a member that already holds a non-default value is left untouched;
        ///     otherwise, <c>false</c> and every settable member is overwritten with a generated value.
        /// </returns>
        /// <remarks>
        ///     This is <c>true</c> by default so that a value already assigned by a constructor or a
        ///     property initializer is preserved, which also keeps a more derived instance assigned to a
        ///     less derived member. A member is considered to hold a non-default value when it differs
        ///     from <c>default</c> for its type, so a reference member that is <c>null</c> and a value
        ///     member that equals its zero value are always generated and cannot be distinguished from an
        ///     unset member.
        /// </remarks>
        public bool RetainAssignedValues { get; set; } = true;
    }
}
