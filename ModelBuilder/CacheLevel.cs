namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="CacheLevel" />
    ///     enum defines the level of caching to use.
    /// </summary>
    public enum CacheLevel
    {
        /// <summary>
        ///     Identifies that no items are stored in a cache.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Identifies that items are stored in a per instance cache.
        /// </summary>
        /// <remarks>This typically means that a cache is valid per Create/Populate call.</remarks>
        PerInstance,

        /// <summary>
        ///     Identifies that items are stored in a cache that lives across Create/Populate calls.
        /// </summary>
        Global
    }
}