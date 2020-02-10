namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="BuildPlan" />
    ///     class is used to identify how <see cref="IExecuteStrategy" /> should operate when using
    ///     <see cref="IBuildProcessor" />.
    /// </summary>
    public class BuildPlan
    {
        /// <summary>
        ///     Gets or sets whether attempts to create the instance requires a constructor to be automatically resolved.
        /// </summary>
        /// <remarks>
        ///     When set to <c>true</c>, detection of the appropriate constructor and creation of constructor parameters will
        ///     be required.
        /// </remarks>
        public bool AutoDetectConstructor { get; set; }

        /// <summary>
        ///     Gets or sets whether properties on the created value should be automatically populated.
        /// </summary>
        public bool AutoPopulate { get; set; }

        /// <summary>
        ///     Gets or sets whether a build action supports populating the created value with its own logic.
        /// </summary>
        public bool SupportsPopulate { get; set; }
    }
}