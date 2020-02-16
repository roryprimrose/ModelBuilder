namespace ModelBuilder
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.BuildActions;
    using ModelBuilder.Properties;
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
        public object Create(Type type, params object[] args)
        {
            return Build(type, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        public void Initialize(IBuildConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public object Populate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            EnsureInitialized();

            var type = instance.GetType();
            var capability =
                _buildProcessor.GetBuildCapability(Configuration, _buildHistory, BuildRequirement.Populate, type);

            _buildHistory.Push(instance);

            try
            {
                instance = Populate(capability, instance, null, null);

                RunPostBuildActions(instance, null);
            }
            finally
            {
                _buildHistory.Pop();
            }

            return instance;
        }

        /// <summary>
        ///     Populates the properties on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        protected virtual object AutoPopulateInstance(object instance, object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            EnsureInitialized();

            var propertyResolver = Configuration.PropertyResolver;
            var type = instance.GetType();

            var propertyInfos = from x in propertyResolver.GetProperties(type)
                orderby GetMaximumOrderPriority(x) descending
                select x;

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyResolver.ShouldPopulateProperty(Configuration, instance, propertyInfo, args))
                {
                    PopulateProperty(propertyInfo, instance, args);
                }
                else
                {
                    Log.IgnoringProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);
                }
            }

            return instance;
        }

        protected virtual object Build(Type type, params object[] arguments)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Build(
                () => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create, type),
                items => _buildProcessor.Build(this, type, items), type, null, arguments);
        }

        protected virtual object Build(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return Build(
                () => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create,
                    parameterInfo),
                items => _buildProcessor.Build(this, parameterInfo, items), parameterInfo.ParameterType,
                parameterInfo.Name, null);
        }

        protected virtual object Build(PropertyInfo propertyInfo, params object[] args)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return Build(
                () => _buildProcessor.GetBuildCapability(Configuration, BuildChain, BuildRequirement.Create,
                    propertyInfo),
                items => _buildProcessor.Build(this, propertyInfo, items), propertyInfo.PropertyType, propertyInfo.Name,
                args);
        }

        protected virtual object Populate(BuildCapability capability, object instance, string referenceName,
            params object[] args)
        {
            Log.PopulatingInstance(instance);

            try
            {
                if (capability.AutoPopulate)
                {
                    // The type creator has indicated that this type should be auto populated by the execute strategy
                    instance = AutoPopulateInstance(instance, args);

                    Debug.Assert(instance != null, "Populating the instance did not return the original instance");
                }

                // Allow the type creator to do its own population of the instance
                return _buildProcessor.Populate(this, instance);
            }
            finally
            {
                Log.PopulatedInstance(instance);
            }
        }

        /// <summary>
        ///     Populates the specified property on the provided instance.
        /// </summary>
        /// <param name="propertyInfo">The property to populate.</param>
        /// <param name="instance">The instance being populated.</param>
        protected virtual void PopulateProperty(PropertyInfo propertyInfo, object instance, params object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            EnsureInitialized();

            if (propertyInfo.GetSetMethod() != null)
            {
                // We can assign to this property
                Log.CreatingProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                var propertyValue = Build(propertyInfo, args);

                propertyInfo.SetValue(instance, propertyValue, null);

                Log.CreatedProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

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

            Populate(existingValue);
        }

        private object Build(Func<BuildCapability> getCapability, Func<object[], object> buildInstance, Type type,
            string referenceName, params object[] args)
        {
            EnsureInitialized();

            var typeToBuild = Configuration.GetBuildType(type, Log);

            var buildChain = BuildChain;

            var context = BuildChain.Last;

            if (typeToBuild == null)
            {
                throw new BuildException(Resources.DefaultBuildStrategy_UndeterminedTargetType, type, null,
                    context, Log.Output);
            }

            var capability = getCapability();

            if (capability == null)
            {
                var message = $"Failed to identify build capabilities for {type.FullName}.";

                throw new BuildException(message, type, referenceName, null, Log.Output);
            }

            Log.CreatingType(typeToBuild, capability.ImplementedByType, context);

            try
            {
                object instance = null;

                if (args?.Length > 0)
                {
                    // We have arguments so will just let the type creator do the work here
                    instance = buildInstance(args);
                }
                else if (capability.AutoDetectConstructor)
                {
                    // Use constructor detection to figure out how to create this instance
                    var constructor = Configuration.ConstructorResolver.Resolve(type);

                    var parameterInfos = constructor.GetParameters();

                    if (parameterInfos.Length == 0)
                    {
                        instance = buildInstance(null);
                    }
                    else
                    {
                        // Get values for each of the constructor parameters
                        var parameters = new Collection<object>();

                        foreach (var parameterInfo in parameterInfos)
                        {
                            var lastContext = buildChain.Last;

                            Log.CreatingParameter(type, parameterInfo.ParameterType, parameterInfo.Name, lastContext);

                            // Recurse to build this parameter value
                            var parameterValue = Build(parameterInfo);

                            parameters.Add(parameterValue);

                            Log.CreatedParameter(type, parameterInfo.ParameterType, parameterInfo.Name, lastContext);
                        }

                        args = parameters.ToArray();

                        instance = buildInstance(args);
                    }
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

                _buildHistory.Push(instance);

                try
                {
                    if (capability.SupportsPopulate)
                    {
                        // Populate the properties
                        instance = Populate(capability, instance, referenceName, args);
                    }

                    RunPostBuildActions(instance, null);
                }
                finally
                {
                    _buildHistory.Pop();
                }

                return instance;
            }
            finally
            {
                Log.CreatedType(typeToBuild, context);
            }
        }

        private void EnsureInitialized()
        {
            if (Configuration == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "The {0} has not be initialized. You must invoke {1} first to provide the build configuration and the build _buildLog.",
                    GetType().FullName,
                    nameof(EnsureInitialized));

                throw new InvalidOperationException(message);
            }
        }

        private int GetMaximumOrderPriority(PropertyInfo property)
        {
            if (Configuration.ExecuteOrderRules == null)
            {
                return 0;
            }

            var matchingRules = from x in Configuration.ExecuteOrderRules
                where x.IsMatch(property)
                orderby x.Priority descending
                select x;
            var matchingRule = matchingRules.FirstOrDefault();

            if (matchingRule == null)
            {
                return 0;
            }

            return matchingRule.Priority;
        }

        private void RunPostBuildActions(object instance, string referenceName)
        {
            var type = instance.GetType();

            var postBuildActions = Configuration.PostBuildActions
                ?.Where(x => x.IsMatch(type, referenceName, _buildHistory)).OrderByDescending(x => x.Priority);

            if (postBuildActions != null)
            {
                foreach (var postBuildAction in postBuildActions)
                {
                    Log.PostBuildAction(type, postBuildAction.GetType(), instance);

                    postBuildAction.Execute(type, referenceName, _buildHistory);
                }
            }
        }

        /// <inheritdoc />
        public IBuildChain BuildChain => _buildHistory;

        /// <inheritdoc />
        public IBuildConfiguration Configuration { get; private set; }

        /// <inheritdoc />
        public IBuildLog Log { get; }
    }
}