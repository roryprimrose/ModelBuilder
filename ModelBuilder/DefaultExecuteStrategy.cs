namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="DefaultExecuteStrategy{T}" />
    ///     class is used to create types and populate instances.
    /// </summary>
    public class DefaultExecuteStrategy : IExecuteStrategy
    {
        private readonly Stack _buildChain = new Stack();

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public object CreateWith(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            EnsureInitialized();

            return Build(type, null, null, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildLog" /> parameter is null.</exception>
        public void Initialize(IBuildConfiguration configuration, IBuildLog buildLog)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (buildLog == null)
            {
                throw new ArgumentNullException(nameof(buildLog));
            }

            Configuration = configuration;
            Log = buildLog;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
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

            _buildChain.Push(instance);

            try
            {
                return PopulateInstance(instance, null);
            }
            finally
            {
                _buildChain.Pop();
            }
        }

        /// <summary>
        ///     Builds an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="context">The possible context object this value is being created for.</param>
        /// <param name="args">The arguements to create the instance with.</param>
        /// <returns>A new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="type" /> parameter can not be created using this strategy.</exception>
        protected virtual object Build(Type type, string referenceName, object context, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            EnsureInitialized();

            var circularReference = BuildChain.FirstOrDefault(x => x.GetType() == type);

            if (circularReference != null)
            {
                Log.CircularReferenceDetected(type);

                return circularReference;
            }

            Func<Type, string, LinkedList<object>, object> generator = null;
            Type generatorType = null;
            var contextType = context?.GetType();
            Type targetType = null;

            // First check if there is a creation rule
            var creationRule =
                Configuration.CreationRules?.Where(x => x.IsMatch(contextType, referenceName))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (creationRule != null)
            {
                generator = creationRule.Create;
                generatorType = creationRule.GetType();

                // The creation rule is targeted against the type that owns the reference
                targetType = contextType;
            }
            else
            {
                // Next check if this is a type supported by a value generator
                var valueGenerator =
                    Configuration.ValueGenerators?.Where(x => x.IsSupported(type, referenceName, BuildChain))
                        .OrderByDescending(x => x.Priority)
                        .FirstOrDefault();

                if (valueGenerator != null)
                {
                    generator = valueGenerator.Generate;
                    generatorType = valueGenerator.GetType();

                    // The value generator is targeted against the type of the reference being generated for
                    targetType = type;
                }
            }

            if (generator != null)
            {
                Log.CreatingValue(type, generatorType, context);

                try
                {
                    return generator(targetType, referenceName, BuildChain);
                }
                catch (BuildException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log.BuildFailure(ex);

                    const string MessageFormat =
                        "Failed to create value for type {0} using value generator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                    var buildLog = Log.Output;
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        MessageFormat,
                        type.FullName,
                        generatorType.FullName,
                        ex.GetType().Name,
                        ex.Message,
                        Environment.NewLine,
                        buildLog);

                    throw new BuildException(message, type, referenceName, context, buildLog, ex);
                }
            }

            var typeCreator =
                Configuration.TypeCreators?.Where(x => x.CanCreate(type, referenceName, BuildChain))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (typeCreator == null)
            {
                throw BuildFailureException(type, referenceName, context);
            }

            Log.CreatingType(type, typeCreator.GetType(), context);

            try
            {
                var instance = CreateAndPopulate(type, referenceName, BuildChain, args, typeCreator);

                return instance;
            }
            catch (BuildException)
            {
                // Don't recapture build failures here
                throw;
            }
            catch (Exception ex)
            {
                Log.BuildFailure(ex);

                const string MessageFormat =
                    "Failed to create type {0} using type creator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                var buildLog = Log.Output;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    MessageFormat,
                    type.FullName,
                    typeCreator.GetType().FullName,
                    ex.GetType().Name,
                    ex.Message,
                    Environment.NewLine,
                    buildLog);

                throw new BuildException(message, type, referenceName, context, buildLog, ex);
            }
            finally
            {
                Log.CreatedType(type, context);
            }
        }

        /// <summary>
        ///     Populates the settable properties on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
        protected virtual object PopulateInstance(object instance, object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            EnsureInitialized();

            Log.PopulatingInstance(instance);

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
                    if (ShouldPopulateProperty(instance, propertyInfo, args))
                    {
                        PopulateProperty(instance, propertyInfo);
                    }
                }

                return instance;
            }
            finally
            {
                Log.PopulatedInstance(instance);
            }
        }

        private Exception BuildFailureException(Type type, string referenceName, object context)
        {
            string message;

            if (string.IsNullOrWhiteSpace(referenceName) == false)
            {
                if (context != null)
                {
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NoMatchingCreatorOrGeneratorFoundWithNameAndContext,
                        type.FullName,
                        referenceName,
                        context.GetType().FullName);
                }
                else
                {
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NoMatchingCreatorOrGeneratorFoundWithName,
                        type.FullName,
                        referenceName);
                }
            }
            else
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.NoMatchingCreatorOrGeneratorFound,
                    type.FullName);
            }

            var ex = new NotSupportedException(message);

            Log.BuildFailure(ex);

            const string MessageFormat =
                "Failed to create instance of type {0}, {1}: {2}{3}{3}At the time of the failure, the build log was:{3}{3}{4}";
            var buildLog = Log.Output;
            var failureMessage = string.Format(
                CultureInfo.CurrentCulture,
                MessageFormat,
                type.FullName,
                ex.GetType().Name,
                ex.Message,
                Environment.NewLine,
                buildLog);

            return new BuildException(failureMessage, type, referenceName, context, buildLog, ex);
        }

        private object CreateAndPopulate(
            Type type,
            string referenceName,
            LinkedList<object> buildChain,
            object[] args,
            ITypeCreator typeCreator)
        {
            var instance = CreateInstance(typeCreator, type, referenceName, buildChain, args);

            if (instance == null)
            {
                return null;
            }

            try
            {
                _buildChain.Push(instance);

                if (typeCreator.AutoPopulate)
                {
                    // The type creator has indicated that this type should be auto populated by the execute strategy
                    instance = PopulateInstance(instance, args);

                    Debug.Assert(instance != null, "Populating the instance did not return the original instance");
                }

                // Allow the type creator to do its own population of the instance
                instance = typeCreator.Populate(instance, this);

                var postBuildActions =
                    Configuration.PostBuildActions?.Where(x => x.IsSupported(type, referenceName, BuildChain))
                        .OrderByDescending(x => x.Priority);

                if (postBuildActions != null)
                {
                    foreach (var postBuildAction in postBuildActions)
                    {
                        Log.PostBuildAction(type, postBuildAction.GetType(), instance);

                        postBuildAction.Execute(type, referenceName, BuildChain);
                    }
                }

                return instance;
            }
            finally
            {
                _buildChain.Pop();
            }
        }

        private object CreateInstance(
            ITypeCreator typeCreator,
            Type type,
            string referenceName,
            LinkedList<object> buildChain,
            object[] args)
        {
            object item;

            if (args?.Length > 0)
            {
                // We have arguments so will just let the type creator do the work here
                item = typeCreator.Create(type, referenceName, buildChain, args);
            }
            else if (typeCreator.AutoDetectConstructor)
            {
                // Use constructor detection to figure out how to create this instance
                var constructor = Configuration.ConstructorResolver.Resolve(type, args);

                var parameterInfos = constructor.GetParameters();

                if (parameterInfos.Length == 0)
                {
                    item = typeCreator.Create(type, referenceName, buildChain);
                }
                else
                {
                    // Get values for each of the constructor parameters
                    var parameters = new Collection<object>();

                    foreach (var parameterInfo in parameterInfos)
                    {
                        var context = buildChain.Last?.Value;

                        Log.CreatingParameter(type, parameterInfo.ParameterType, parameterInfo.Name, context);

                        // Recurse to build this parameter value
                        var parameterValue = Build(parameterInfo.ParameterType, parameterInfo.Name, null);

                        parameters.Add(parameterValue);

                        Log.CreatedParameter(type, parameterInfo.ParameterType, parameterInfo.Name, context);
                    }

                    item = typeCreator.Create(type, referenceName, buildChain, parameters.ToArray());
                }
            }
            else
            {
                // The type creator is going to be solely responsible for creating this instance
                item = typeCreator.Create(type, referenceName, buildChain);
            }

            return item;
        }

        private void EnsureInitialized()
        {
            if (Configuration == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "The {0} has not be initialized. You must invoke {1} first to provide the build configuration and the build log.",
                    GetType().FullName,
                    MethodBase.GetCurrentMethod().Name);

                throw new InvalidOperationException(message);
            }
        }

        private int GetMaximumOrderPrority(Type type, string propertyName)
        {
            if (Configuration.ExecuteOrderRules == null)
            {
                return 0;
            }

            var matchingRules = from x in Configuration.ExecuteOrderRules
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

        /// <summary>
        /// Determines whether the property should be populated with a value.
        /// </summary>
        /// <param name="instance">The instance being populated.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns><c>true</c> if the property should be populated; otherwise <c>false</c>.</returns>
        protected virtual bool ShouldPopulateProperty(object instance, PropertyInfo propertyInfo, object[] args)
        {
            var type = instance.GetType();

            // Check if there is a matching ignore rule
            var ignoreRule =
                Configuration.IgnoreRules?.FirstOrDefault(
                    x => x.TargetType.IsAssignableFrom(type) && (x.PropertyName == propertyInfo.Name));

            if (ignoreRule != null)
            {
                Log.IgnoringProperty(propertyInfo.PropertyType, propertyInfo.Name, ignoreRule.GetType(), instance);

                // We need to ignore this property
                return false;
            }

            if (args == null)
            {
                // No constructor arguments
                // Assume that constructor has not defined a value for this property
                return true;
            }

            if (args.Length == 0)
            {
                // No constructor arguments
                // Assume that constructor has not defined a value for this property
                return true;
            }

            var matchingParameterTypes =
                args.Where(x => x != null && propertyInfo.PropertyType.IsInstanceOfType(x)).ToList();

            if (matchingParameterTypes.Count == 0)
            {
                // There are no constructor types that match the property type
                // Assume that no constructor parameter has defined this value
                return true;
            }

            var propertyValue = propertyInfo.GetValue(instance, null);
            var defaultValue = GetDefaultValue(propertyInfo.PropertyType);

            if (propertyValue == defaultValue)
            {
                // The property matches the default value of its type
                // A constructor parameter could have assigned the default type value or no constructor parameter
                // was assigned to the property
                // In either case we want to build a value for this property
                return true;
            }

            var instanceParameters = matchingParameterTypes.Where(x => x.GetType().IsValueType == false);

            if (instanceParameters.Any(x => ReferenceEquals(x, propertyValue)))
            {
                // The constructor parameter matches the property value
                // We don't want to overwrite this
                return false;
            }
            
            var valueParameters = matchingParameterTypes.Where(x => x.GetType().IsValueType).ToList();

            if (valueParameters.Count == 0)
            {
                // There are no value type constructor parameters to try to check against this property
                // Build a new value for this property
                return true;
            }

            // Get the constructor matching the arguments so that we can try to match constructor parameter names against the property name
            var constructor = Configuration.ConstructorResolver.Resolve(type, args);
            var parameters = constructor.GetParameters();

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];

                if (propertyInfo.PropertyType.IsInstanceOfType(parameter.ParameterType) == false)
                {
                    // The constructor parameter type does not match the property value, keep looking
                    continue;
                }

                var parameterValue = args[index];

                if (parameterValue != propertyValue)
                {
                    // This constructor parameter does not match property value, keep looking
                    continue;
                }

                if (string.Equals(propertyInfo.Name, parameter.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // We have found that the property name and type are equivalent and the value is not the default value
                    // This is good enough to assume that the property value came from the constructor and we should not overwrite it
                    return false;
                }
            }

            return true;
        }

        private static object GetDefaultValue(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        private void PopulateProperty(object instance, PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetSetMethod(true).IsPublic)
            {
                Log.CreatingProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                var parameterValue = Build(propertyInfo.PropertyType, propertyInfo.Name, instance);

                propertyInfo.SetValue(instance, parameterValue, null);

                Log.CreatedProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                return;
            }

            // The property is read-only
            // We need to try to populate the property using a type creator that can populate it
            // To determine the correct type creator, we will use the type of the property instance value
            // rather than the property type because it may be more accurate
            var value = propertyInfo.GetValue(instance, null);

            if (value == null)
            {
                // We don't have a value to work with
                return;
            }

            var propertyType = value.GetType();

            // Attempt to find a type creator for this type that will help us figure out how it should be populated
            var typeCreator =
                Configuration.TypeCreators?.Where(x => x.CanPopulate(propertyType, propertyInfo.Name, BuildChain))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (typeCreator == null)
            {
                // This is either not supported or it is a value type
                return;
            }

            // The property is public, but the setter is not
            // We might still be able to populate this instance if it has a value
            var originalValue = propertyInfo.GetValue(instance, null);

            if (typeCreator.AutoPopulate)
            {
                // The type creator has indicated that this type should be auto populated by the execute strategy
                PopulateInstance(originalValue, null);
            }

            // Allow the type creator to do its own population of the instance
            typeCreator.Populate(originalValue, this);
        }

        /// <inheritdoc />
        public LinkedList<object> BuildChain
        {
            get
            {
                var chain = new LinkedList<object>();

                foreach (var item in _buildChain)
                {
                    chain.AddFirst(item);
                }

                return chain;
            }
        }

        /// <inheritdoc />
        public IBuildConfiguration Configuration { get; private set; }

        /// <inheritdoc />
        public IBuildLog Log { get; private set; }
    }
}