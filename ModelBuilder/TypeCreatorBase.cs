namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using ModelBuilder.Properties;
    using ModelBuilder.TypeCreators;

    /// <summary>
    ///     The <see cref="TypeCreatorBase" />
    ///     class is used to provide the common implementation of a type creator.
    /// </summary>
    public abstract class TypeCreatorBase : ITypeCreator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        public virtual bool CanCreate(Type type, string referenceName, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsInterface)
            {
                return false;
            }

            if (type.IsAbstract)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        public virtual bool CanPopulate(Type type, string referenceName, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        public virtual object Create(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (executeStrategy.BuildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            VerifyCreateRequest(type, referenceName, executeStrategy);

            return CreateInstance(type, referenceName, executeStrategy, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        public virtual object Populate(object instance, IExecuteStrategy executeStrategy)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (executeStrategy.BuildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            VerifyPopulateRequest(instance.GetType(), null, executeStrategy);

            // The default will be to not do any additional population of the instance
            return PopulateInstance(instance, executeStrategy);
        }

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        protected abstract object CreateInstance(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args);

        /// <summary>
        ///     Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The populated instance.</returns>
        protected abstract object PopulateInstance(object instance, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Verifies that the minimum required information has been provided in order to create an instance.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyCreateRequest(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            // Calculate the build chain just once
            var buildChain = executeStrategy.BuildChain;

            if (buildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            if (CanCreate(type, referenceName, buildChain))
            {
                return;
            }

            var message = string.Format(
                CultureInfo.CurrentCulture,
                Resources.Error_GenerationNotSupportedFormat,
                GetType().FullName,
                type.FullName,
                referenceName ?? "<null>");

            throw new NotSupportedException(message);
        }

        /// <summary>
        ///     Verifies that the minimum required information has been provided in order to populate an instance.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyPopulateRequest(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            // Calculate the build chain just once
            var buildChain = executeStrategy.BuildChain;

            if (buildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            if (CanPopulate(type, referenceName, buildChain))
            {
                return;
            }

            var message = string.Format(
                CultureInfo.CurrentCulture,
                Resources.Error_GenerationNotSupportedFormat,
                GetType().FullName,
                type.FullName,
                referenceName ?? "<null>");

            throw new NotSupportedException(message);
        }

        /// <inheritdoc />
        public virtual bool AutoDetectConstructor => true;

        /// <inheritdoc />
        public virtual bool AutoPopulate => true;

        /// <inheritdoc />
        public virtual int Priority { get; } = 0;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}