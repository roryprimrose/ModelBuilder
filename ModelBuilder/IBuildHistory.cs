namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IBuildHistory" />
    ///     interface defines the members for adding and removing items from the build chain.
    /// </summary>
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