namespace ModelBuilder.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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

            return source.Select(x => new[] {x[0], x[1]}).ToList();
        }
    }
}