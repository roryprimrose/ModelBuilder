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
        /// <param name="type">The type of value to build.</param>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <returns>The new value.</returns>
        object Build(Type type, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="parameterInfo">The parameter of value to build.</param>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <returns>The new value.</returns>
        object Build(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="propertyInfo">The property of value to build.</param>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <returns>The new value.</returns>
        object Build(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy);
    }
}