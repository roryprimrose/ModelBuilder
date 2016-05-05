using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IBuildLog"/>
    /// interface defines the build operations that can be logged.
    /// </summary>
    public interface IBuildLog
    {
        /// <summary>
        /// Clears the current build log.
        /// </summary>
        void Clear();

        /// <summary>
        /// Logs that a type has been created.
        /// </summary>
        /// <param name="type">The type created.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatedType(Type type, object context);

        /// <summary>
        /// Logs that a parameter is being created for a constructor.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="parameterType">The parameter type being created.</param>
        /// <param name="parameterName">The name of the constructor parameter.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreateParameter(Type instanceType, Type parameterType, string parameterName, object context);

        /// <summary>
        /// Logs that a property is being created for an instance.
        /// </summary>
        /// <param name="propertyType">The property type being created.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreateProperty(Type propertyType, string propertyName, object context);

        /// <summary>
        /// Logs that a type is being created.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingType(Type type, object context);

        /// <summary>
        /// Logs that a value is being created.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        void CreatingValue(Type type, object context);

        /// <summary>
        /// Logs that an instance has been populated.
        /// </summary>
        /// <param name="instance">The instance that has been populated.</param>
        void PopulatedInstance(object instance);

        /// <summary>
        /// Logs that an instance is being populated.
        /// </summary>
        /// <param name="instance">The instance being populated.</param>
        void PopulatingInstance(object instance);

        /// <summary>
        /// Gets the output of the build log.
        /// </summary>
        string Output { get; }
    }
}