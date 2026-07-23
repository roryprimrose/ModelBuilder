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
    internal sealed class ValueSourceRegistry
    {
        private readonly Dictionary<Type, object> _sources = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<IBuildContext, object?>> _boxedFactories = new Dictionary<Type, Func<IBuildContext, object?>>();

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
            _boxedFactories[typeof(T)] = context => source.Create(context, new BuildTarget(typeof(T), typeof(T).Name));
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
        ///     Attempts to create a value for a runtime <see cref="Type" /> using the value source
        ///     registered for it, without requiring the type to be known at compile time.
        /// </summary>
        /// <param name="type">The type to create a value for.</param>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="value">The created value, boxed when the type is a value type.</param>
        /// <returns><c>true</c> if a value source is registered for <paramref name="type" />; otherwise, <c>false</c>.</returns>
        internal bool TryCreateBoxed(Type type, IBuildContext context, out object? value)
        {
            if (_boxedFactories.TryGetValue(type, out var factory))
            {
                value = factory(context);

                return true;
            }

            value = null;

            return false;
        }

        /// <summary>
        ///     Gets the types that have a registered value source.
        /// </summary>
        public IReadOnlyCollection<Type> RegisteredTypes => _sources.Keys;
    }
}
