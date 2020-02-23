namespace ModelBuilder
{
    using System;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="DefaultExecuteStrategy{T}" />
    ///     class is used to create and populate <typeparamref name="T" /> instances.
    /// </summary>
    /// <typeparam name="T">The type of instance to create and populate.</typeparam>
    public class DefaultExecuteStrategy<T> : DefaultExecuteStrategy, IExecuteStrategy<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExecuteStrategy" /> class.
        /// </summary>
        public DefaultExecuteStrategy()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExecuteStrategy" /> class.
        /// </summary>
        /// <param name="buildHistory">The build history tracker.</param>
        /// <param name="buildLog">The build log.</param>
        /// <param name="buildProcessor">The build processor.</param>
        public DefaultExecuteStrategy(IBuildHistory buildHistory, IBuildLog buildLog, IBuildProcessor buildProcessor) : base(buildHistory, buildLog, buildProcessor)
        {
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public virtual T Create(params object[] args)
        {
            var requestedType = typeof(T);

            var instance = Build(requestedType, args);

            if (instance == null)
            {
                return default;
            }

            return (T) instance;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public virtual T Populate(T instance)
        {
            return (T) Populate((object) instance);
        }
    }
}