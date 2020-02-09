namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IBuildProcessor" />
    ///     interface defines the members for building a new value.
    /// </summary>
    public interface IBuildProcessor
    {
        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="type">The type of value to build.</param>
        /// <param name="arguments">The constructor parameters to create the instance with.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="parameterInfo">The parameter of value to build.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="propertyInfo">The property of value to build.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo);
    }
}