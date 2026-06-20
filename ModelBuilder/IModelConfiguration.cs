namespace ModelBuilder
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     The <see cref="IModelConfiguration" /> interface
    ///     is the fluent configuration returned by the <see cref="Model" /> facade. It accumulates
    ///     ignore rules, type mappings, configuration modules and a log sink for a single build, then
    ///     produces the configured instance through <see cref="Create{T}" /> or <see cref="Populate{T}" />.
    /// </summary>
    public interface IModelConfiguration
    {
        /// <summary>
        ///     Creates a populated instance of <typeparamref name="T" /> using this configuration.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <returns>The created instance.</returns>
        T Create<T>();

        /// <summary>
        ///     Adds an ignore rule for the specified member.
        /// </summary>
        /// <typeparam name="T">The type that declares the member.</typeparam>
        /// <param name="expression">An expression selecting the member to ignore.</param>
        /// <returns>The same configuration for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> does not select a member.</exception>
        IModelConfiguration Ignoring<T>(Expression<Func<T, object?>> expression);

        /// <summary>
        ///     Adds a type mapping from a source type to a concrete target type.
        /// </summary>
        /// <typeparam name="TSource">The source type, typically an interface or abstract type.</typeparam>
        /// <typeparam name="TTarget">The concrete type to build in its place.</typeparam>
        /// <returns>The same configuration for chaining.</returns>
        IModelConfiguration Mapping<TSource, TTarget>()
            where TTarget : TSource;

        /// <summary>
        ///     Begins a typed construction of <typeparamref name="T" /> using this configuration. Supply
        ///     constructor arguments through the generated <c>From</c> extension method on the returned
        ///     handle.
        /// </summary>
        /// <typeparam name="T">The type to construct.</typeparam>
        /// <returns>A handle whose generated <c>From</c> overloads name the constructors of <typeparamref name="T" />.</returns>
        Construction<T> Construct<T>();

        /// <summary>
        ///     Registers a custom value source for every member and root of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <returns>The same configuration for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> parameter is <c>null</c>.</exception>
        IModelConfiguration AddValueSource<T>(IValueSource<T> source);

        /// <summary>
        ///     Registers a custom value source for members of type <typeparamref name="T" /> whose name
        ///     matches one of <paramref name="memberNames" />.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <param name="memberNames">The member names the source matches.</param>
        /// <returns>The same configuration for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="source" /> or <paramref name="memberNames" /> parameter is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="memberNames" /> parameter is empty or contains a null, empty or
        ///     whitespace entry.
        /// </exception>
        IModelConfiguration AddValueSource<T>(IValueSource<T> source, params string[] memberNames);

        /// <summary>
        ///     Populates an existing instance of <typeparamref name="T" /> using this configuration.
        /// </summary>
        /// <typeparam name="T">The type to populate.</typeparam>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        T Populate<T>(T instance);

        /// <summary>
        ///     Applies a configuration module to this configuration.
        /// </summary>
        /// <typeparam name="TModule">The configuration module to apply.</typeparam>
        /// <returns>The same configuration for chaining.</returns>
        IModelConfiguration UsingModule<TModule>()
            where TModule : IConfigurationModule, new();

        /// <summary>
        ///     Captures the structured build log and writes it to the supplied sink after the build.
        /// </summary>
        /// <param name="sink">The action that receives the rendered build log.</param>
        /// <returns>The same configuration for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sink" /> parameter is <c>null</c>.</exception>
        IModelConfiguration WriteLog(Action<string> sink);
    }
}
