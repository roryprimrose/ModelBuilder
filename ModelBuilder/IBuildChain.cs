namespace ModelBuilder
{
    using System.Collections.Generic;

    public interface IBuildChain : IEnumerable<object>
    {
        int Count { get; }

        object First { get; }

        object Last { get; }
    }
}