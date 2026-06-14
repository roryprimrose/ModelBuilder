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
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The created instance.</returns>
        T Create<T>(params object?[]? args);

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
