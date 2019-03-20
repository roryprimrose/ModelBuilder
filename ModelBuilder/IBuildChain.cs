namespace ModelBuilder
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IBuildChain" />
    ///     interface defines the properties that allow inspection of build chain values.
    /// </summary>
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