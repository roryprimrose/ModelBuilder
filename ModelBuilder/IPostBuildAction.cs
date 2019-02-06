namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IPostBuildAction" />
    ///     interface defines the members for running post-build actions on models created by <see cref="IExecuteStrategy" />.
    /// </summary>
    public interface IPostBuildAction
    {
        /// <summary>
        ///     Executes the post build action and returns the result.
        /// </summary>
        /// <param name="type">The type of value to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        void Execute(Type type, string referenceName, IBuildChain buildChain);

        /// <summary>
        ///     Returns whether the specified type is supported by this type.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if the type is supported; otherwise <c>false</c>.</returns>
        bool IsSupported(Type type, string referenceName, IBuildChain buildChain);

        /// <summary>
        ///     Gets the priority for this type.
        /// </summary>
        int Priority { get; }
    }
}