namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.BuildActions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="DefaultExecuteStrategy{T}" />
    ///     class is used to create types and populate instances.
    /// </summary>
    public class DefaultExecuteStrategy : IExecuteStrategy
    {
        private readonly IBuildHistory _buildHistory;
        private readonly IBuildProcessor _buildProcessor;
        private IBuildConfiguration? _configuration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExecuteStrategy" /> class.
        /// </summary>
        public DefaultExecuteStrategy() : this(new BuildHistory(), new DefaultBuildLog(), new BuildProcessor())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExecuteStrategy" /> class.
        /// </summary>
        /// <param name="buildHistory">The build history tracker.</param>
        /// <param name="buildLog">The build log.</param>
        /// <param name="buildProcessor">The build processor.</param>
        public DefaultExecuteStrategy(IBuildHistory buildHistory, IBuildLog buildLog, IBuildProcessor buildProcessor)
        {
            _buildHistory = buildHistory ?? throw new ArgumentNullException(nameof(buildHistory));
            Log = buildLog ?? throw new ArgumentNullException(nameof(buildLog));
            _buildProcessor = buildProcessor ?? throw new ArgumentNullException(nameof(buildProcessor));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public object Create(Type type, params object?[]? args)
        {
            return Build(type, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        public void Initialize(IBuildConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public virtual object Populate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var capability = _buildProcessor.GetBuildCapability(Configuration, _buildHistory, BuildRequirement.Populate,
                instance.GetType());

            if (capability == null)
            {
                return instance;
            }

            var type = instance.GetType();

            var populatedInstance = Populate(capability, instance);

            if (populatedInstance == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture, "The type '{0}' failed to return a non-null value of type '{1}' after populating its properties.", capability.ImplementedByType.FullName, type.FullName);

                throw new BuildException(message, type, null, null, Log.Output);
            }

            return populatedInstance;
        }

        /// <summary>
        ///     Builds a value for the specified type.
        /// </summary>
        /// <param name="type">The type of value to build.</param>
        /// <param name="args">The arguments used to create the value.</param>
        /// <returns>The value created.</returns>
        protected virtual object Build(Type type, params object?[]? args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            BuildCapability? GetCapability() => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create, type);

            var instance = Build(
                GetCapability,
                items => _buildProcessor.Build(this, type, items), type, null, args)!;

            if (instance == null)
            {
                // The Build method above would have thrown an exception if the build capability could not be identified
                var capability = GetCapability()!;

                var message = string.Format(CultureInfo.CurrentCulture, "The type '{0}' failed to create a non-null value of type '{1}'", capability.ImplementedByType.FullName, type.FullName);

                throw new BuildException(message, type, null, null, Log.Output);
            }

            RunPostBuildActions(instance, type);

            return instance;
        }

        /// <summary>
        ///     Builds a value for the specified parameter.
        /// </summary>
        /// <param name="parameterInfo">The parameter to build a value for.</param>
        /// <returns>The value created for the parameter.</returns>
        protected virtual object? Build(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var instance = Build(
                () => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create,
                    parameterInfo),
                items => _buildProcessor.Build(this, parameterInfo, items), parameterInfo.ParameterType,
                parameterInfo.Name, null);

            if (instance == null)
            {
                return instance;
            }

            RunPostBuildActions(instance, parameterInfo);

            return instance;
        }

        /// <summary>
        ///     Builds a value for the specified property.
        /// </summary>
        /// <param name="propertyInfo">The property to build a value for.</param>
        /// <param name="args">The arguments used to create the parent instance.</param>
        /// <returns>The value created for the property.</returns>
        protected virtual object? Build(PropertyInfo propertyInfo, params object?[]? args)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var instance = Build(
                () => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create,
                    propertyInfo),
                items => _buildProcessor.Build(this, propertyInfo, items), propertyInfo.PropertyType, propertyInfo.Name,
                args);

            if (instance == null)
            {
                return instance;
            }

            RunPostBuildActions(instance, propertyInfo);

            return instance;
        }

        /// <summary>
        ///     Populates the specified property on the provided instance.
        /// </summary>
        /// <param name="propertyInfo">The property to populate.</param>
        /// <param name="instance">The instance being populated.</param>
        /// <param name="args">The arguments used to create <paramref name="instance" />.</param>
        protected virtual void PopulateProperty(PropertyInfo propertyInfo, object instance,
            params object?[]? args)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (propertyInfo.GetSetMethod() != null)
            {
                // We can assign to this property
                Log.CreatingProperty(propertyInfo, instance);

                var propertyValue = Build(propertyInfo, args);

                propertyInfo.SetValue(instance, propertyValue, null);

                Log.CreatedProperty(propertyInfo, instance);

                return;
            }

            // The property is read-only
            // Because of prior filtering, we should have a property that is a reference type that we can populate
            var existingValue = propertyInfo.GetValue(instance, null);

            if (existingValue == null)
            {
                // We don't have a value to work with
                return;
            }

            var capability = _buildProcessor.GetBuildCapability(Configuration, _buildHistory, BuildRequirement.Populate,
                existingValue.GetType());

            if (capability != null)
            {
                Populate(capability, existingValue, args);
            }

            // This object was never created here but was populated
            // Run post build actions against it so that they can be applied against this existing instance
            RunPostBuildActions(existingValue, propertyInfo);
        }

        private void AutoPopulateInstance(object instance, object?[]? args)
        {
            var propertyResolver = Configuration.PropertyResolver;
            var type = instance.GetType();

            var propertyInfos = propertyResolver.GetOrderedProperties(Configuration, type);

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyResolver.IsIgnored(Configuration, instance, propertyInfo, args))
                {
                    Log.IgnoringProperty(propertyInfo, instance);
                }
                else
                {
                    PopulateProperty(propertyInfo, instance);
                }
            }
        }

        private object? Build(Func<BuildCapability?> getCapability, Func<object?[]?, object?> buildInstance, Type type,
            string? referenceName, params object?[]? args)
        {
            var capability = getCapability();

            if (capability == null)
            {
                var message = $"Failed to identify build capabilities for {type.FullName}.";

                throw new BuildException(message, type, referenceName, null, Log.Output);
            }

            var context = _buildHistory.Last;

            Log.CreatingType(type, capability.ImplementedByType, context);

            try
            {
                object? instance;

                if (args?.Length > 0)
                {
                    // We have arguments so will just let the type creator do the work here
                    instance = buildInstance(args);
                }
                else if (capability.AutoDetectConstructor)
                {
                    var parameters = CreateParameterValues(type);

                    instance = buildInstance(parameters);
                }
                else
                {
                    // The type creator is going to be solely responsible for creating this instance
                    instance = buildInstance(null);
                }

                if (instance == null)
                {
                    return null;
                }

                if (capability.SupportsPopulate == false
                    && instance.GetType() != type)
                {
                    // Get the capability again on the instance type
                    // The scenario here is that an instance has been created with a different type
                    // where the build action didn't support populating the original type
                    // It has however created a different type that may support population
                    // The example here is IEnumerable<T> which may be built as something like List<T>
                    capability = _buildProcessor.GetBuildCapability(Configuration, _buildHistory,
                        BuildRequirement.Create,
                        instance.GetType());
                }

                // Populate the properties
                instance = Populate(capability!, instance, args);

                return instance;
            }
            finally
            {
                Log.CreatedType(type, context);
            }
        }

        private object?[]? CreateParameterValues(Type type)
        {
            // Resolve the type being created
            var typeToCreate = Configuration.TypeResolver.GetBuildType(Configuration, type);

            // Use constructor detection to figure out how to create this instance
            var constructorResolver = Configuration.ConstructorResolver;

            var constructor = constructorResolver.Resolve(typeToCreate);

            if (constructor == null)
            {
                // This should be a struct that only has a default constructor
                // In this case the type does not have any registered constructor (no default)
                // There are no parameters for the default creation of a struct
                return null;
            }

            var parameterInfos = constructorResolver.GetOrderedParameters(Configuration, constructor).ToList();

            if (parameterInfos.Count <= 0)
            {
                return null;
            }

            // Create an ExpandoObject to hold the parameter values as we build them
            // ValueGenerators can use these parameters (expressed as properties) to assist in 
            // building values that are dependent on other values
            IDictionary<string, object?> propertyWrapper = new ExpandoObject();

            _buildHistory.Push(propertyWrapper);

            try
            {
                foreach (var parameterInfo in parameterInfos)
                {
                    var lastContext = _buildHistory.Last;

                    Log.CreatingParameter(parameterInfo, lastContext);

                    // Recurse to build this parameter value
                    var parameterValue = Build(parameterInfo);

                    propertyWrapper[parameterInfo.Name] = parameterValue;

                    Log.CreatedParameter(parameterInfo, lastContext);
                }
            }
            finally
            {
                _buildHistory.Pop();
            }

            var originalParameters = constructor.GetParameters();
            var parameterValues = new Collection<object?>();

            // Re-order the parameters back into the order expected by the constructor
            foreach (var parameterInfo in originalParameters)
            {
                var parameterValue = propertyWrapper[parameterInfo.Name];

                parameterValues.Add(parameterValue);
            }

            var parameters = parameterValues.ToArray();

            return parameters;
        }

        private object Populate(BuildCapability capability, object instance, params object?[]? args)
        {
            if (capability.SupportsPopulate == false)
            {
                return instance;
            }

            _buildHistory.Push(instance);
            Log.PopulatingInstance(instance);

            try
            {
                if (capability.AutoPopulate)
                {
                    // The type creator has indicated that this type should be auto populated by the execute strategy
                    AutoPopulateInstance(instance, args);
                }

                // Allow the type creator to do its own population of the instance
                return _buildProcessor.Populate(this, instance);
            }
            finally
            {
                Log.PopulatedInstance(instance);
                _buildHistory.Pop();
            }
        }

        private void RunPostBuildActions(object instance, Type type)
        {
            var postBuildActions = Configuration.PostBuildActions
                ?.Where(x => x.IsMatch(_buildHistory, type)).OrderByDescending(x => x.Priority);

            if (postBuildActions != null)
            {
                foreach (var postBuildAction in postBuildActions)
                {
                    Log.PostBuildAction(type, postBuildAction.GetType(), instance);

                    postBuildAction.Execute(_buildHistory, instance, type);
                }
            }
        }

        private void RunPostBuildActions(object instance, ParameterInfo parameterInfo)
        {
            var postBuildActions = Configuration.PostBuildActions
                ?.Where(x => x.IsMatch(_buildHistory, parameterInfo)).OrderByDescending(x => x.Priority);

            if (postBuildActions != null)
            {
                foreach (var postBuildAction in postBuildActions)
                {
                    Log.PostBuildAction(parameterInfo.ParameterType, postBuildAction.GetType(), instance);

                    postBuildAction.Execute(_buildHistory, instance, parameterInfo);
                }
            }
        }

        private void RunPostBuildActions(object instance, PropertyInfo propertyInfo)
        {
            var postBuildActions = Configuration.PostBuildActions
                ?.Where(x => x.IsMatch(_buildHistory, propertyInfo)).OrderByDescending(x => x.Priority);

            if (postBuildActions != null)
            {
                foreach (var postBuildAction in postBuildActions)
                {
                    Log.PostBuildAction(propertyInfo.PropertyType, postBuildAction.GetType(), instance);

                    postBuildAction.Execute(_buildHistory, instance, propertyInfo);
                }
            }
        }

        /// <inheritdoc />
        public IBuildChain BuildChain => _buildHistory;

        /// <inheritdoc />
        public IBuildConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        "The {0} has not be initialized. You must invoke {1} first to provide the build configuration.",
                        GetType().FullName,
                        nameof(Initialize));

                    throw new InvalidOperationException(message);
                }

                return _configuration;
            }
        }

        /// <inheritdoc />
        public IBuildLog Log { get; }
    }
}