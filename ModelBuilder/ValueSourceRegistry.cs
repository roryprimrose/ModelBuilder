namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ValueSourceRegistry" /> class
    ///     maps a runtime <see cref="Type" /> to its value source for the non-generic and polymorphic
    ///     dispatch paths. The source reference is stored as <see cref="object" />, but the produced
    ///     value is never boxed because the source is invoked through the typed
    ///     <see cref="IValueSource{T}" />.
    /// </summary>
    public sealed class ValueSourceRegistry
    {
        private readonly Dictionary<Type, object> _sources = new Dictionary<Type, object>();

        /// <summary>
        ///     Registers a value source for the type it produces.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> parameter is <c>null</c>.</exception>
        public void Register<T>(IValueSource<T> source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            _sources[typeof(T)] = source;
        }

        /// <summary>
        ///     Attempts to resolve the value source registered for <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to resolve a value source for.</typeparam>
        /// <param name="source">The registered value source, when one exists.</param>
        /// <returns><c>true</c> if a value source is registered; otherwise, <c>false</c>.</returns>
        public bool TryGet<T>(out IValueSource<T>? source)
        {
            if (_sources.TryGetValue(typeof(T), out var stored))
            {
                source = (IValueSource<T>)stored;

                return true;
            }

            source = null;

            return false;
        }

        /// <summary>
        ///     Gets the types that have a registered value source.
        /// </summary>
        public IReadOnlyCollection<Type> RegisteredTypes => _sources.Keys;
    }
}
