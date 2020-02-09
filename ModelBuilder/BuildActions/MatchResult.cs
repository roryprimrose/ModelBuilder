namespace ModelBuilder.BuildActions
{
    using System;

    /// <summary>
    ///     The <see cref="MatchResult" />
    ///     class is used to identify the a match between a create request and a <see cref="IBuildAction" /> along with
    ///     identifying the capabilities of the build action.
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        ///     Gets a standard result for where there is no match.
        /// </summary>
        public static readonly MatchResult NoMatch = new MatchResult();

        /// <summary>
        ///     Gets or sets whether properties on the created value should be automatically populated.
        /// </summary>
        public bool AutoPopulate { get; set; }

        /// <summary>
        ///     Gets or sets whether there the <see cref="IBuildAction" /> supports the requested scenario.
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        ///     Gets or sets whether attempts to create the instance requires a constructor to be called via
        ///     <see cref="Activator.CreateInstance(System.Type)" />.
        /// </summary>
        /// <remarks>
        ///     When set to <c>true</c>, detection of the appropriate constructor and creation of constructor parameters will
        ///     be required.
        /// </remarks>
        public bool RequiresActivator { get; set; }

        /// <summary>
        ///     Gets or sets whether a build action supports populating the created value with its own logic.
        /// </summary>
        public bool SupportsPopulate { get; set; }
    }
}