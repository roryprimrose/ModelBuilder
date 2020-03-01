namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RegexTypeNameValueGenerator" />
    ///     class is used to generate a value for a target type and property/parameter name.
    /// </summary>
    public abstract class RegexTypeNameValueGenerator : ValueGeneratorBase
    {
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
        protected override bool IsMatch(Type type, string referenceName, IBuildChain buildChain)
        {
            if (_type.IsAssignableFrom(type) == false)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(referenceName))
            {
                return false;
            }

            if (_nameExpression.IsMatch(referenceName))
            {
                return true;
            }

            return false;
        }
    }
}