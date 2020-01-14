namespace ModelBuilder.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [SuppressMessage(
        "Code.Quality",
        "CA1710",
        Justification = "The data source not have the characteristics of a Collection.")]
    public class NumericTypeDataSource : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = BuildValues();

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static List<object[]> BuildValues()
        {
            var source = new NumericTypeRangeDataSource();

            return source.Select(
                x => new[]
                {
                    x[0], x[1]
                }).ToList();
        }
    }
}