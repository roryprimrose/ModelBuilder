namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="DelegateValueSource{T}" /> class
    ///     adapts a factory delegate into an <see cref="IValueSource{T}" />, used to register the
    ///     built-in value sources without a dedicated class per type.
    /// </summary>
    /// <typeparam name="T">The type the source produces.</typeparam>
    public sealed class DelegateValueSource<T> : IValueSource<T>
    {
        private readonly Func<BuildContext, BuildTarget, T> _factory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateValueSource{T}" /> class.
        /// </summary>
        /// <param name="factory">The factory invoked to produce a value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory" /> parameter is <c>null</c>.</exception>
        public DelegateValueSource(Func<BuildContext, T> factory)
        {
            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _factory = (context, _) => factory(context);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateValueSource{T}" /> class.
        /// </summary>
        /// <param name="factory">The factory invoked with the build target to produce a value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory" /> parameter is <c>null</c>.</exception>
        public DelegateValueSource(Func<BuildContext, BuildTarget, T> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc />
        public T Create(BuildContext context, in BuildTarget target)
        {
            return _factory(context, target);
        }
    }
}
