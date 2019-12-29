namespace ModelBuilder
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="IBuildChain" />
    ///     interface defines the properties that allow inspection of build chain values.
    /// </summary>
    [SuppressMessage(
        "Code.Quality",
        "CA1710",
        Justification = "The build chain is enumerable, but does not have the characteristics of a Collection.")]
    public interface IBuildChain : IEnumerable<object>
    {
        /// <summary>
        ///     Gets the number of items in the build chain.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Get the first item added to the build chain.
        /// </summary>
        object First { get; }

        /// <summary>
        ///     Gets the last item added to the build chain.
        /// </summary>
        object Last { get; }
    }
}