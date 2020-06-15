namespace ModelBuilder.BuildActions
{
    using System;
    using System.Reflection;
    using ModelBuilder.CreationRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildCapability" />
    ///     class is used to identify how <see cref="IExecuteStrategy" /> should operate when using
    ///     <see cref="IBuildProcessor" />.
    /// </summary>
    public class BuildCapability : IBuildCapability
    {
        private readonly Func<IExecuteStrategy, ParameterInfo, object?[]?, object?> _createParameter;
        private readonly Func<IExecuteStrategy, PropertyInfo, object?[]?, object?> _createProperty;
        private readonly Func<IExecuteStrategy, Type, object?[]?, object?> _createType;
        private readonly Func<IExecuteStrategy, object, object> _populate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildCapability" /> class.
        /// </summary>
        /// <param name="generator">The generator that provides the build functions.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        public BuildCapability(IValueGenerator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            ImplementedByType = generator.GetType();
            SupportsCreate = true;

            _createType = (strategy, type, args) => generator.Generate(strategy, type);
            _createProperty = (strategy, propertyInfo, args) => generator.Generate(strategy, propertyInfo);
            _createParameter = (strategy, parameterInfo, args) => generator.Generate(strategy, parameterInfo);
            _populate = (strategy, instance) =>
                throw new NotSupportedException(
                    $"{nameof(IValueGenerator)} types do not support populating an instance.");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildCapability" /> class.
        /// </summary>
        /// <param name="rule">The rule that provides the build functions.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public BuildCapability(ICreationRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            ImplementedByType = rule.GetType();
            SupportsCreate = true;

            _createType = (strategy, type, args) => rule.Create(strategy, type);
            _createProperty = (strategy, propertyInfo, args) => rule.Create(strategy, propertyInfo);
            _createParameter = (strategy, parameterInfo, args) => rule.Create(strategy, parameterInfo);
            _populate = (strategy, instance) =>
                throw new NotSupportedException(
                    $"{nameof(ICreationRule)} types do not support populating an instance.");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildCapability" /> class.
        /// </summary>
        /// <param name="typeCreator">The type creator that provides the build functions.</param>
        /// <param name="supportsCreate">
        ///     <c>true</c> if the <paramref name="typeCreator" /> can create an instance; otherwise
        ///     <c>false</c>.
        /// </param>
        /// <param name="supportsPopulate">
        ///     <c>true</c> if the <paramref name="typeCreator" /> can populate an instance; otherwise
        ///     <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreator" /> parameter is <c>null</c>.</exception>
        public BuildCapability(ITypeCreator typeCreator, bool supportsCreate, bool supportsPopulate)
        {
            if (typeCreator == null)
            {
                throw new ArgumentNullException(nameof(typeCreator));
            }

            ImplementedByType = typeCreator.GetType();
            SupportsCreate = supportsCreate;
            SupportsPopulate = supportsPopulate;
            AutoPopulate = typeCreator.AutoPopulate;

            _createType = typeCreator.Create;
            _createProperty = typeCreator.Create;
            _createParameter = typeCreator.Create;
            _populate = typeCreator.Populate;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object? CreateParameter(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo,
            object?[]? args)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return _createParameter(executeStrategy, parameterInfo, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object? CreateProperty(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo,
            object?[]? args)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _createProperty(executeStrategy, propertyInfo, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        public object? CreateType(IExecuteStrategy executeStrategy, Type targetType, object?[]? args)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            return _createType(executeStrategy, targetType, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return _populate(executeStrategy, instance);
        }

        /// <inheritdoc />
        public bool AutoPopulate { get; }

        /// <inheritdoc />
        public Type ImplementedByType { get; }

        /// <inheritdoc />
        public bool SupportsCreate { get; }

        /// <inheritdoc />
        public bool SupportsPopulate { get; }
    }
}