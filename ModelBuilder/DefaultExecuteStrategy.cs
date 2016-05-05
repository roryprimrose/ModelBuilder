﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DefaultExecuteStrategy{T}"/>
    /// class is used to create and populate <typeparamref name="T"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of instance to create and populate.</typeparam>
    public class DefaultExecuteStrategy<T> : IExecuteStrategy<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultExecuteStrategy{T}"/> class.
        /// </summary>
        public DefaultExecuteStrategy()
        {
            BuildStrategy = new DefaultBuildStrategy();
        }

        /// <inheritdoc />
        public virtual T CreateWith(params object[] args)
        {
            var requestedType = typeof(T);

            var instance = Build(requestedType, null, null, args);

            if (instance == null)
            {
                // We can't populate a null instance
                return default(T);
            }

            return (T) instance;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public object CreateWith(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Build(type, null, null, args);
        }

        /// <inheritdoc />
        public virtual T Populate(T instance)
        {
            return (T) PopulateInstance(instance);
        }

        /// <inheritdoc />
        public object Populate(object instance)
        {
            return PopulateInstance(instance);
        }

        /// <summary>
        /// Builds an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="context">The possible context object this value is being created for.</param>
        /// <param name="args">The arguements to create the instance with.</param>
        /// <returns>A new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="type"/> parameter can not be created using this strategy.</exception>
        protected virtual object Build(Type type, string referenceName, object context, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // First check if this is a type supported by a value generator
            var valueGenerator =
                BuildStrategy.ValueGenerators.Where(x => x.IsSupported(type, referenceName, context))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (valueGenerator != null)
            {
                BuildStrategy.BuildLog.CreatingValue(type, context);

                return valueGenerator.Generate(type, referenceName, context);
            }

            BuildStrategy.BuildLog.CreatingType(type, context);

            try
            {
                var typeCreator =
                    BuildStrategy.TypeCreators.Where(x => x.IsSupported(type, referenceName, context))
                        .OrderByDescending(x => x.Priority)
                        .FirstOrDefault();

                if (typeCreator == null)
                {
                    string message;

                    if (context != null)
                    {
                        message = string.Format(CultureInfo.CurrentCulture,
                            Resources.NoMatchingCreatorOrGeneratorFoundWithNameAndContext,
                            type.FullName, referenceName, context.GetType().FullName);
                    }
                    else if (string.IsNullOrWhiteSpace(referenceName) == false)
                    {
                        message = string.Format(CultureInfo.CurrentCulture,
                            Resources.NoMatchingCreatorOrGeneratorFoundWithName,
                            type.FullName, referenceName);
                    }
                    else
                    {
                        message = string.Format(CultureInfo.CurrentCulture, Resources.NoMatchingCreatorOrGeneratorFound,
                            type.FullName);
                    }

                    throw new NotSupportedException(message);
                }

                var instance = CreateInstance(typeCreator, type, referenceName, context, args);

                if (instance == null)
                {
                    return null;
                }

                if (typeCreator.AutoPopulate)
                {
                    // The type creator has indicated that this type should not be auto populated by the execute strategy
                    instance = PopulateInstance(instance);

                    Debug.Assert(instance != null, "Populating the instance did not return the original instance");
                }

                // Allow the type creator to do its own population of the instance
                instance = typeCreator.Populate(instance, this);

                return instance;
            }
            finally
            {
                BuildStrategy.BuildLog.CreatedType(type, context);
            }
        }

        /// <summary>
        /// Populates the settable properties on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> parameter is null.</exception>
        protected virtual object PopulateInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            BuildStrategy.BuildLog.PopulatingInstance(instance);

            try
            {
                // We will only set public instance properties that have a setter (the setter must also be public)
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
                var type = instance.GetType();

                var propertyInfos = from x in type.GetProperties(flags)
                    where x.CanWrite
                    orderby GetMaximumOrderPrority(x.PropertyType, x.Name) descending
                    select x;

                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.SetMethod.IsPublic == false)
                    {
                        // The property is public, but the setter is not
                        continue;
                    }

                    // Check if there is a matching ignore rule
                    if (
                        BuildStrategy.IgnoreRules.Any(
                            x => x.TargetType.IsAssignableFrom(type) && x.PropertyName == propertyInfo.Name))
                    {
                        // We need to ignore this property
                        continue;
                    }

                    BuildStrategy.BuildLog.CreateProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                    var parameterValue = Build(propertyInfo.PropertyType, propertyInfo.Name, instance);

                    propertyInfo.SetValue(instance, parameterValue);
                }

                return instance;
            }
            finally
            {
                BuildStrategy.BuildLog.PopulatedInstance(instance);
            }
        }

        private object CreateInstance(ITypeCreator typeCreator, Type type, string referenceName, object context,
            object[] args)
        {
            object item;

            if (args?.Length > 0)
            {
                // We have arguments so will just let the type creator do the work here
                item = typeCreator.Create(type, referenceName, context, args);
            }
            else if (typeCreator.AutoDetectConstructor)
            {
                // Use constructor detection to figure out how to create this instance
                var constructor = BuildStrategy.ConstructorResolver.Resolve(type, args);

                var parameterInfos = constructor.GetParameters();

                if (parameterInfos.Length == 0)
                {
                    item = typeCreator.Create(type, referenceName, context);
                }
                else
                {
                    // Get values for each of the constructor parameters
                    var parameters = new Collection<object>();

                    foreach (var parameterInfo in parameterInfos)
                    {
                        BuildStrategy.BuildLog.CreateParameter(type, parameterInfo.ParameterType, parameterInfo.Name,
                            context);

                        // Recurse to build this parameter value
                        var parameterValue = Build(parameterInfo.ParameterType, parameterInfo.Name, null);

                        parameters.Add(parameterValue);
                    }

                    item = typeCreator.Create(type, referenceName, context, parameters.ToArray());
                }
            }
            else
            {
                // The type creator is going to be solely responsible for creating this instance
                item = typeCreator.Create(type, referenceName, context);
            }

            return item;
        }

        private int GetMaximumOrderPrority(Type type, string propertyName)
        {
            var matchingRules = from x in BuildStrategy.ExecuteOrderRules
                where x.IsMatch(type, propertyName)
                orderby x.Priority descending
                select x;
            var matchingRule = matchingRules.FirstOrDefault();

            if (matchingRule == null)
            {
                return 0;
            }

            return matchingRule.Priority;
        }

        /// <inheritdoc />
        public IBuildStrategy BuildStrategy { get; set; }
    }
}