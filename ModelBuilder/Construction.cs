namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="Construction{T}" /> struct
    ///     is the typed-construction handle returned by <see cref="Model.Construct{T}" /> and
    ///     <see cref="IModelConfiguration.Construct{T}" />. It builds nothing on its own; a generated
    ///     <c>From</c> extension method supplies the constructor arguments and drives the build through
    ///     <see cref="Execute" />.
    /// </summary>
    /// <typeparam name="T">The type to construct.</typeparam>
    /// <remarks>
    ///     The struct carries the build configuration and log sink accumulated by the fluent
    ///     configuration so that mappings, custom value sources, ignore rules and logging apply to the
    ///     constructed object and its whole subgraph. Generated <c>From</c> extension methods are keyed
    ///     on this receiver type, so the editor offers only the constructors of <typeparamref name="T" />.
    /// </remarks>
    [SuppressMessage(
        "Usage",
        "CA1815:Override equals and operator equals on value types",
        Justification = "Construction<T> is a transient build handle that is never compared for equality.")]
    public readonly struct Construction<T>
    {
        private readonly BuildConfiguration? _configuration;
        private readonly Action<string>? _logSink;

        internal Construction(BuildConfiguration? configuration, Action<string>? logSink)
        {
            _configuration = configuration;
            _logSink = logSink;
        }

        /// <summary>
        ///     Drives a build using the supplied construction callback, applying the configuration and
        ///     log sink this handle carries.
        /// </summary>
        /// <param name="build">
        ///     The callback that constructs and populates the instance using the supplied build context.
        ///     Generated <c>From</c> extension methods provide this callback.
        /// </param>
        /// <returns>The constructed and populated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="build" /> parameter is <c>null</c>.</exception>
        /// <remarks>
        ///     This method is the single public seam through which generated code starts a build from the
        ///     consuming assembly. It is not intended to be called directly.
        /// </remarks>
        public T Execute(Func<IBuildContext, T> build)
        {
            build = build ?? throw new ArgumentNullException(nameof(build));

            if (_logSink == null)
            {
                return Model.ConstructWith(build, _configuration, null);
            }

            var log = new BuildLog();

            try
            {
                return Model.ConstructWith(build, _configuration, log);
            }
            finally
            {
                _logSink(log.Render());
            }
        }
    }
}
