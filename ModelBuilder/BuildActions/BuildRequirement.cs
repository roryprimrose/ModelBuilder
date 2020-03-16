namespace ModelBuilder.BuildActions
{
    /// <summary>
    ///     The <see cref="BuildRequirement" />
    ///     enum identifies a required build capability.
    /// </summary>
    public enum BuildRequirement
    {
        /// <summary>
        ///     Identifies that a creation build process is required.
        /// </summary>
        Create,

        /// <summary>
        ///     Identifies that a populate build process is required.
        /// </summary>
        Populate
    }
}