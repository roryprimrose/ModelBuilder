namespace ModelBuilder
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="IBuildHistory" />
    ///     interface defines the members for adding and removing items from the build chain.
    /// </summary>
    [SuppressMessage("Code.Quality", "CA1710", Justification = "The history is enumerable, but does not have the characteristics of a Collection.")]
    public interface IBuildHistory : IBuildChain
    {
        /// <summary>
        ///     Removes the last item added to the build chain.
        /// </summary>
        void Pop();

        /// <summary>
        ///     Tracks the specified item in the build chain.
        /// </summary>
        /// <param name="instance">The item to track.</param>
        void Push(object instance);
    }
}