namespace ModelBuilder
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
        public T? Create(IBuildContext context, in BuildTarget target)
        {
            // The engine always supplies the concrete BuildContext; the cast reaches the internal
            // null-percentage and value-source resolution that are not part of the public contract.
            var ctx = (BuildContext)context;

            if (ctx.Random.NextInt32(0, 99) < ctx.NullPercentage)
            {
                return null;
            }

            if (ctx.TryResolveValueSource<T>(out var inner) == false || inner == null)
            {
                return null;
            }

            return inner.Create(ctx, in target);
        }
    }
}
