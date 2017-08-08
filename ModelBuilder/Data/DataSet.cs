namespace ModelBuilder.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The <see cref="DataSet{T}" />
    ///     class provides common logic to getting a random item of the data set.
    /// </summary>
    /// <typeparam name="T">The type of item in the data set.</typeparam>
    public class DataSet<T> : ReadOnlyCollection<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataSet{T}" /> class.
        /// </summary>
        /// <param name="list">The list of items for the data set.</param>
        public DataSet(IList<T> list) : base(list)
        {
        }

        /// <summary>
        ///     Returns a random item from the data set.
        /// </summary>
        /// <returns>A new data item.</returns>
        public T Next()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, Count - 1);

            return this[index];
        }
    }
}