﻿namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IBuildLog" />
    ///     interface defines the build operations that can be logged.
    /// </summary>
    public interface IBuildLog
    {
        /// <summary>
        ///     Logs a build failure
        /// </summary>
        /// <param name="ex">The expception to log.</param>
        void BuildFailure(Exception ex);

        /// <summary>
        ///     Logs that a circular reference was detected in the build chain.
        /// </summary>
        /// <param name="type">The type to create.</param>
        void CircularReferenceDetected(Type type);

        /// <summary>
        ///     Clears the current build log.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Logs that a parameter has been created for a constructor.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="parameterType">The parameter type being created.</param>
        /// <param name="parameterName">The name of the constructor parameter.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatedParameter(Type instanceType, Type parameterType, string parameterName, object context);

        /// <summary>
        ///     Logs that a property has been created for an instance.
        /// </summary>
        /// <param name="propertyType">The property type being created.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatedProperty(Type propertyType, string propertyName, object context);

        /// <summary>
        ///     Logs that a type has been created.
        /// </summary>
        /// <param name="type">The type created.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatedType(Type type, object context);

        /// <summary>
        ///     Logs that a parameter is being created for a constructor.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="parameterType">The parameter type being created.</param>
        /// <param name="parameterName">The name of the constructor parameter.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingParameter(Type instanceType, Type parameterType, string parameterName, object context);

        /// <summary>
        ///     Logs that a property is being created for an instance.
        /// </summary>
        /// <param name="propertyType">The property type being created.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingProperty(Type propertyType, string propertyName, object context);

        /// <summary>
        ///     Logs that a type is being created.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="creatorType">The type of creator used to create the type.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingType(Type type, Type creatorType, object context);

        /// <summary>
        ///     Logs that a value is being created.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="generatorType">The type of generator used to create the value.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingValue(Type type, Type generatorType, object context);

        /// <summary>
        ///     Logs that a property is being ignored for an instance.
        /// </summary>
        /// <param name="propertyType">The property type being ignored.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void IgnoringProperty(Type propertyType, string propertyName, object context);

        /// <summary>
        ///     Logs that an instance has been populated.
        /// </summary>
        /// <param name="instance">The instance that has been populated.</param>
        void PopulatedInstance(object instance);

        /// <summary>
        ///     Logs that an instance is being populated.
        /// </summary>
        /// <param name="instance">The instance being populated.</param>
        void PopulatingInstance(object instance);

        /// <summary>
        ///     Logs that a type is running through post build actions.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="postBuildType">The type of the post build action.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void PostBuildAction(Type type, Type postBuildType, object context);

        /// <summary>
        ///     Gets the output of the build log.
        /// </summary>
        string Output { get; }
    }
}