namespace ModelBuilder.BuildActions
{
    /// <summary>
    ///     The <see cref="MatchResult" />
    ///     class is used to identify the a match between a create request and a <see cref="IBuildAction" /> along with
    ///     identifying the capabilities of the build action.
    /// </summary>
    public class MatchResult : BuildPlan
    {
        /// <summary>
        ///     Gets a standard result for where there is no match.
        /// </summary>
        public static readonly MatchResult NoMatch = new MatchResult();

        /// <summary>
        ///     Gets or sets whether there the <see cref="IBuildAction" /> supports the requested scenario.
        /// </summary>
        public bool IsMatch { get; set; }
    }
}