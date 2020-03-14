namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IPostBuildAction" />
    ///     interface defines the members for running post-build actions on models created by <see cref="IExecuteStrategy" />.
    /// </summary>
    public interface IPostBuildAction
    {
        /// <summary>
        ///     Executes the post build action for an object created by type.
        /// </summary>
        /// <param name="instance">The object that was built.</param>
        /// <param name="type">The type of value to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        void Execute(object instance, Type type, IBuildChain buildChain);

        /// <summary>
        ///     Executes the post build action for an object created by parameter.
        /// </summary>
        /// <param name="instance">The object that was built.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        void Execute(object instance, ParameterInfo parameterInfo, IBuildChain buildChain);

        /// <summary>
        ///     Executes the post build action for an object create by property.
        /// </summary>
        /// <param name="instance">The object that was built.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        void Execute(object instance, PropertyInfo propertyInfo, IBuildChain buildChain);

        /// <summary>
        ///     Returns whether the specified type matches this action.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if the type matches this action; otherwise <c>false</c>.</returns>
        bool IsMatch(Type type, IBuildChain buildChain);

        /// <summary>
        ///     Returns whether the specified parameter matches this action.
        /// </summary>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if the type matches this action; otherwise <c>false</c>.</returns>
        bool IsMatch(ParameterInfo parameterInfo, IBuildChain buildChain);

        /// <summary>
        ///     Returns whether the specified property matches this action.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if the type matches this action; otherwise <c>false</c>.</returns>
        bool IsMatch(PropertyInfo propertyInfo, IBuildChain buildChain);

        /// <summary>
        ///     Gets the priority for this type.
        /// </summary>
        int Priority { get; }
    }
}