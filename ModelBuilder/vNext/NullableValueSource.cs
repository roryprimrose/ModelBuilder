namespace ModelBuilder.vNext
{
    /// <summary>
    ///     The <see cref="NullableValueSource{T}" /> class
    ///     adapts the value source for an underlying value type into a source for its
    ///     <see cref="System.Nullable{T}" />, returning <c>null</c> according to the build context's
    ///     null percentage and otherwise delegating to the underlying source.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    public sealed class NullableValueSource<T> : IValueSource<T?>
        where T : struct
    {
        /// <inheritdoc />
        public T? Create(BuildContext context, in BuildTarget target)
        {
            if (context.Random.NextInt32(0, 99) < context.NullPercentage)
            {
                return null;
            }

            var inner = ValueSource<T>.Instance;

            if (inner == null)
            {
                return null;
            }

            return inner.Create(context, in target);
        }
    }
}
