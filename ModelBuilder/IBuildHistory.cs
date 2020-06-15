namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ModelBuilder.BuildActions;

    /// <summary>
    ///     The <see cref="IBuildHistory" />
    ///     interface defines the members for adding and removing items from the build chain.
    /// </summary>
    [SuppressMessage(
        "Code.Quality",
        "CA1710",
        Justification = "The history is enumerable, but does not have the characteristics of a Collection.")]
    public interface IBuildHistory : IBuildChain
    {
        /// <summary>
        ///     Adds a build capability for the specified type.
        /// </summary>
        /// <param name="type">The type being created.</param>
        /// <param name="capability">The build capability.</param>
        void AddCapability(Type type, IBuildCapability capability);

        /// <summary>
        ///     Gets the build capability for the specified type.
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <returns>The build capability or <c>null</c> if no capability exists for the type.</returns>
        IBuildCapability? GetCapability(Type type);

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