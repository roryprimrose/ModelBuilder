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
                return AutoPopulateInstance(instance, null);
            }
            finally
            {
                _buildChain.Pop();
            }
        }

        /// <summary>
        ///     Populates the properties on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
        protected virtual object AutoPopulateInstance(object instance, object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            EnsureInitialized();

            var type = instance.GetType();
            var propertyResolver = Configuration.PropertyResolver;

            var propertyInfos = from x in type.GetProperties()
                                where propertyResolver.CanPopulate(x)
                                orderby GetMaximumOrderPrority(x.PropertyType, x.Name) descending
                                select x;

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyResolver.ShouldPopulateProperty(Configuration, instance, propertyInfo, args))
                {
                    PopulateProperty(instance, propertyInfo);
                }
                else
                {
                    Log.IgnoringProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);
                }
            }

            return instance;
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

            Func<Type, string, IExecuteStrategy, object> generator = null;
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
                    return generator(targetType, referenceName, this);
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
        ///     Populates the specified property on the provided instance.
        /// </summary>
        /// <param name="instance">The instance being populated.</param>
        /// <param name="propertyInfo">The property to populate.</param>
        protected virtual void PopulateProperty(object instance, PropertyInfo propertyInfo)
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

                var parameterValue = Build(propertyInfo.PropertyType, propertyInfo.Name, instance);

                propertyInfo.SetValue(instance, parameterValue, null);

                Log.CreatedProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                return;
            }

            // The property is read-only
            // Because of prior filtering, we should have a property that is a reference type that we can populate
            var value = propertyInfo.GetValue(instance, null);

            if (value == null)
            {
                // We don't have a value to work with
                return;
            }

            // We need to try to populate the property using a type creator that can populate it
            // To determine the correct type creator, we will use the type of the property instance value
            // rather than the property type because it may be more accurate due to inheritance chains
            var propertyType = value.GetType();

            // Attempt to find a type creator for this type that will help us figure out how it should be populated
            var typeCreator =
                Configuration.TypeCreators?.Where(x => x.CanPopulate(propertyType, propertyInfo.Name, BuildChain))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (typeCreator == null)
            {
                // This is either not supported or it is a value type
                throw BuildFailureException(propertyType, propertyInfo.Name, instance);
            }

            // The property is public, but the setter is not
            // We might still be able to populate this instance if it has a value
            var originalValue = propertyInfo.GetValue(instance, null);

            PopulateInternal(propertyType, propertyInfo.Name, null, typeCreator, originalValue);
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
            var outcome = CreateInstance(typeCreator, type, referenceName, buildChain, args);

            var instance = outcome.Item1;
            var arguments = outcome.Item2;

            if (instance == null)
            {
                return null;
            }

            _buildChain.Push(instance);

            try
            {
                return PopulateInternal(type, referenceName, arguments, typeCreator, instance);
            }
            finally
            {
                _buildChain.Pop();
            }
        }

        private Tuple<object, object[]> CreateInstance(
            ITypeCreator typeCreator,
            Type type,
            string referenceName,
            LinkedList<object> buildChain,
            object[] args)
        {
            object instance;

            if (args?.Length > 0)
            {
                // We have arguments so will just let the type creator do the work here
                instance = typeCreator.Create(type, referenceName, this, args);

                return new Tuple<object, object[]>(instance, args);
            }

            if (typeCreator.AutoDetectConstructor)
            {
                // Use constructor detection to figure out how to create this instance
                var constructor = Configuration.ConstructorResolver.Resolve(type);

                var parameterInfos = constructor.GetParameters();

                if (parameterInfos.Length == 0)
                {
                    instance = typeCreator.Create(type, referenceName, this);

                    return new Tuple<object, object[]>(instance, args);
                }

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

                var arguments = parameters.ToArray();

                instance = typeCreator.Create(type, referenceName, this, arguments);

                return new Tuple<object, object[]>(instance, arguments);
            }

            // The type creator is going to be solely responsible for creating this instance
            instance = typeCreator.Create(type, referenceName, this);

            return new Tuple<object, object[]>(instance, args);
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

        private object PopulateInternal(
            Type type,
            string referenceName,
            object[] args,
            ITypeCreator typeCreator,
            object instance)
        {
            Log.PopulatingInstance(instance);

            try
            {
                if (typeCreator.AutoPopulate)
                {
                    // The type creator has indicated that this type should be auto populated by the execute strategy
                    instance = AutoPopulateInstance(instance, args);

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
                Log.PopulatedInstance(instance);
            }
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