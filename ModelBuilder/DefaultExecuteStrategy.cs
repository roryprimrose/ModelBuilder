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
    using Properties;

    /// <summary>
    ///     The <see cref="DefaultExecuteStrategy{T}" />
    ///     class is used to create and populate <typeparamref name="T" /> instances.
    /// </summary>
    /// <typeparam name="T">The type of instance to create and populate.</typeparam>
    public class DefaultExecuteStrategy<T> : IExecuteStrategy<T>
    {
        private readonly Stack _buildChain = new Stack();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExecuteStrategy{T}" /> class.
        /// </summary>
        public DefaultExecuteStrategy()
        {
            // Use the current global build strategy
            BuildStrategy = Model.BuildStrategy;
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public virtual T CreateWith(params object[] args)
        {
            var requestedType = typeof(T);

            var instance = Build(requestedType, null, null, args);

            if (instance == null)
            {
                // We can't populate a null instance
                return default(T);
            }

            return (T)instance;
        }

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

            return Build(type, null, null, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">
        ///     No <see cref="IValueGenerator" /> or <see cref="ITypeCreator" /> was found to
        ///     generate a requested type.
        /// </exception>
        /// <exception cref="BuildException">Failed to generate a requested type.</exception>
        public virtual T Populate(T instance)
        {
            return (T)Populate((object)instance);
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

            _buildChain.Push(instance);

            try
            {
                return PopulateInstance(instance);
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

            var circularReference = BuildChain.FirstOrDefault(x => x.GetType() == type);

            if (circularReference != null)
            {
                BuildStrategy.BuildLog.CircularReferenceDetected(type);

                return circularReference;
            }

            Func<Type, string, LinkedList<object>, object> generator = null;
            Type generatorType = null;
            var contextType = context?.GetType();
            Type targetType = null;

            // First check if there is a creation rule
            var creationRule =
                BuildStrategy.CreationRules?.Where(x => x.IsMatch(contextType, referenceName))
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
                    BuildStrategy.ValueGenerators?.Where(x => x.IsSupported(type, referenceName, BuildChain))
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
                BuildStrategy.BuildLog.CreatingValue(type, context);

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
                    BuildStrategy.BuildLog.BuildFailure(ex);

                    const string messageFormat =
                        "Failed to create value for type {0} using value generator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                    var buildLog = BuildStrategy.BuildLog.Output;
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        messageFormat,
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
                BuildStrategy.TypeCreators?.Where(x => x.CanCreate(type, referenceName, BuildChain))
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();

            if (typeCreator == null)
            {
                throw BuildFailureException(type, referenceName, context);
            }

            BuildStrategy.BuildLog.CreatingType(type, context);

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
                BuildStrategy.BuildLog.BuildFailure(ex);

                const string messageFormat =
                    "Failed to create type {0} using type creator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                var buildLog = BuildStrategy.BuildLog.Output;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    messageFormat,
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
                BuildStrategy.BuildLog.CreatedType(type, context);
            }
        }

        /// <summary>
        ///     Populates the settable properties on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
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
                    if (propertyInfo.GetSetMethod(true).IsPublic == false)
                    {
                        // The property is public, but the setter is not
                        continue;
                    }

                    // Check if there is a matching ignore rule
                    var ignoreRule = BuildStrategy.IgnoreRules?.FirstOrDefault(
                        x => x.TargetType.IsAssignableFrom(type) && (x.PropertyName == propertyInfo.Name));

                    if (ignoreRule != null)
                    {
                        // We need to ignore this property
                        continue;
                    }

                    BuildStrategy.BuildLog.CreateProperty(propertyInfo.PropertyType, propertyInfo.Name, instance);

                    var parameterValue = Build(propertyInfo.PropertyType, propertyInfo.Name, instance);

                    propertyInfo.SetValue(instance, parameterValue, null);
                }

                return instance;
            }
            finally
            {
                BuildStrategy.BuildLog.PopulatedInstance(instance);
            }
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
                    // The type creator has indicated that this type should not be auto populated by the execute strategy
                    instance = PopulateInstance(instance);

                    Debug.Assert(instance != null, "Populating the instance did not return the original instance");
                }

                // Allow the type creator to do its own population of the instance
                instance = typeCreator.Populate(instance, this);

                var postBuildActions = BuildStrategy.PostBuildActions
                    ?.Where(x => x.IsSupported(type, referenceName, BuildChain))
                    .OrderByDescending(x => x.Priority);

                if (postBuildActions != null)
                {
                    foreach (var postBuildAction in postBuildActions)
                    {
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
                var constructor = BuildStrategy.ConstructorResolver.Resolve(type, args);

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

                        BuildStrategy.BuildLog.CreateParameter(
                            type,
                            parameterInfo.ParameterType,
                            parameterInfo.Name,
                            context);

                        // Recurse to build this parameter value
                        var parameterValue = Build(parameterInfo.ParameterType, parameterInfo.Name, null);

                        parameters.Add(parameterValue);
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

        private int GetMaximumOrderPrority(Type type, string propertyName)
        {
            if (BuildStrategy.ExecuteOrderRules == null)
            {
                return 0;
            }

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

            BuildStrategy.BuildLog.BuildFailure(ex);

            const string messageFormat =
                "Failed to create instance of type {0}, {1}: {2}{3}{3}At the time of the failure, the build log was:{3}{3}{4}";
            var buildLog = BuildStrategy.BuildLog.Output;
            var failureMessage = string.Format(
                CultureInfo.CurrentCulture,
                messageFormat,
                type.FullName,
                ex.GetType().Name,
                ex.Message,
                Environment.NewLine,
                buildLog);

            return new BuildException(failureMessage, type, referenceName, context, buildLog, ex);
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
        public IBuildStrategy BuildStrategy { get; set; }
    }
}