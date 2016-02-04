using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

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
            TypeCreators = new List<ITypeCreator>();
            ValueGenerators = new List<IValueGenerator>();
            IgnoreRules = new List<IgnoreRule>();
            ConstructorResolver = new DefaultConstructorResolver();
        }

        /// <inheritdoc />
        public virtual T CreateWith(params object[] args)
        {
            var requestedType = typeof (T);

            var instance = BuildInstance(requestedType, args);

            if (instance == null)
            {
                // We can't populate a null instance
                return default(T);
            }

            var item = (T) instance;

            return Populate(item);
        }

        /// <inheritdoc />
        public virtual T Populate(T instance)
        {
            var requestedType = typeof (T);

            return (T) PopulateInstance(requestedType, instance);
        }

        /// <summary>
        /// Builds an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="args">The arguements to create the instance with.</param>
        /// <returns>A new instance.</returns>
        protected virtual object BuildInstance(Type type, params object[] args)
        {
            // First check if this is a type supported by a value generator
            var valueGenerator = ValueGenerators.FirstOrDefault(x => x.IsSupported(type));

            if (valueGenerator != null)
            {
                return valueGenerator.Generate(type);
            }

            var typeCreator = TypeCreators.FirstOrDefault(x => x.IsSupported(type));

            if (typeCreator == null)
            {
                throw new NotSupportedException(
                    "No supporting ITypeCreator or IValueGenerator was found for the requested type.");
            }

            object item;

            if (args?.Length > 0)
            {
                // We have arguments so will just let the type creator do the work here
                item = typeCreator.Create(type, args);
            }
            else if (typeCreator.AutoDetectConstructor)
            {
                // Use constructor detection to figure out how to create this instance
                var constructor = ConstructorResolver.Resolve(type, args);

                var parameterInfos = constructor.GetParameters();

                if (parameterInfos.Length == 0)
                {
                    item = typeCreator.Create(type);
                }
                else
                {
                    // Get values for each of the constructor parameters
                    var parameters = new Collection<object>();

                    foreach (var parameterInfo in parameterInfos)
                    {
                        // Recurse to build this parameter value
                        var parameterValue = BuildInstance(parameterInfo.ParameterType);

                        parameters.Add(parameterValue);
                    }

                    item = typeCreator.Create(type, parameters.ToArray());
                }
            }
            else
            {
                // The type creator is going to be solely responsible for creating this instance
                item = typeCreator.Create(type);
            }

            return item;
        }

        /// <summary>
        /// Populates the settable properties on the specified instance.
        /// </summary>
        /// <param name="type">The type of instance to populate.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        protected virtual object PopulateInstance(Type type, object instance)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;

            var propertyInfos = type.GetProperties(flags).Where(x => x.CanWrite);

            foreach (var propertyInfo in propertyInfos)
            {
                // Check if there is a matching ignore rule
                if (IgnoreRules.Any(x => x.TargetType == type && x.PropertyName == propertyInfo.Name))
                {
                    // We need to ignore this property
                    continue;
                }

                var parameterValue = BuildInstance(propertyInfo.PropertyType);

                propertyInfo.SetValue(instance, parameterValue);
            }

            return instance;
        }

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; set; }

        /// <inheritdoc />
        public ICollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}