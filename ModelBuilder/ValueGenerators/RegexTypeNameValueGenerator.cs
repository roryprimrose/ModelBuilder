namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RegexTypeNameValueGenerator" />
    ///     class is used to generate a value for a target type and property/parameter name.
    /// </summary>
    public abstract class RegexTypeNameValueGenerator : IValueGenerator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        private readonly Regex _nameExpression;
        private readonly Type _type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexTypeNameValueGenerator" /> class.
        /// </summary>
        /// <param name="nameExpression">The regular expression that matches the target name.</param>
        /// <param name="type">The type of value to generate.</param>
        protected RegexTypeNameValueGenerator(Regex nameExpression, Type type)
        {
            _nameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <inheritdoc />
        public virtual object Generate(Type type, IExecuteStrategy executeStrategy)
        {
            // These value generators to not support constructors
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual object Generate(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var type = propertyInfo.PropertyType;
            var name = propertyInfo.Name;

            return Generate(type, name, executeStrategy);
        }

        /// <inheritdoc />
        public virtual object Generate(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var type = parameterInfo.ParameterType;
            var name = parameterInfo.Name;

            return Generate(type, name, executeStrategy);
        }

        // TODO: Make this protected abstract once all old usages have been removed
        /// <summary>
        ///     Generates a value for the specified type and reference name.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">The name of the property or parameter to generate the value for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns></returns>
        public abstract object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy);

        /// <inheritdoc />
        public virtual bool IsSupported(Type type, IBuildChain buildChain)
        {
            // These value generators to not support constructors
            return false;
        }

        /// <inheritdoc />
        public virtual bool IsSupported(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var type = propertyInfo.PropertyType;
            var name = propertyInfo.Name;

            return IsSupported(type, name);
        }

        /// <inheritdoc />
        public virtual bool IsSupported(ParameterInfo parameterInfo, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var type = parameterInfo.ParameterType;
            var name = parameterInfo.Name;

            return IsSupported(type, name);
        }

        /// <inheritdoc />
        public bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
        {
            throw new NotImplementedException();
        }

        private bool IsSupported(Type type, string name)
        {
            if (_type.IsAssignableFrom(type) == false)
            {
                return false;
            }

            if (_nameExpression.IsMatch(name))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public abstract int Priority { get; }

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}