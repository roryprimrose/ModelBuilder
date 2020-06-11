namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IExecuteStrategy" />
    ///     interface defines the dependencies used to create and populate instances.
    /// </summary>
    public interface IExecuteStrategy
    {
        /// <summary>
        ///     Creates a new instance of the specified type with optional constructor arguments.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">The constructor arguments of the type.</param>
        /// <returns>A new instance of the specified type.</returns>
        object Create(Type type, params object?[]? args);

        /// <summary>
        ///     Creates a set of parameters for the specified method.
        /// </summary>
        /// <param name="method">The method that defines the parameters.</param>
        /// <returns>An array of parameter values.</returns>
        object?[]? CreateParameters(MethodBase method);

        /// <summary>
        ///     Initializes the execute strategy with a build configuration.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        void Initialize(IBuildConfiguration configuration);

        /// <summary>
        ///     Populates values onto settable properties of the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated object.</returns>
        object Populate(object instance);

        /// <summary>
        ///     Gets the build chain for objects up to the current build execution.
        /// </summary>
        IBuildChain BuildChain { get; }

        /// <summary>
        ///     Gets the build configuration.
        /// </summary>
        IBuildConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the build log for items created by this strategy.
        /// </summary>
        IBuildLog Log { get; }
    }
}