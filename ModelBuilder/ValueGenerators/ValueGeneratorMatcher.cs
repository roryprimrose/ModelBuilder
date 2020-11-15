namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="ValueGeneratorMatcher" />
    ///     class is used to provide the common logic for evaluating whether a generator matches a target to generate for.
    /// </summary>
    public abstract class ValueGeneratorMatcher : ValueGeneratorBase
    {
        private readonly Func<Type, string?, IBuildChain, bool> _matcher;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueGeneratorMatcher" /> class.
        /// </summary>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="types" /> parameter is <c>null</c>.</exception>
        protected ValueGeneratorMatcher(params Type[] types)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));

            _matcher = (type, referenceName, context) => { return types.Any(x => x == type); };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueGeneratorMatcher" /> class.
        /// </summary>
        /// <param name="referenceName">Identifies the possible parameter or property name the generator can match.</param>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="referenceName" /> parameter is <c>null</c>.</exception>
        protected ValueGeneratorMatcher(string referenceName, params Type[] types)
        {
            referenceName = referenceName ?? throw new ArgumentNullException(nameof(referenceName));

            _matcher = (type, name, context) =>
            {
                if (name == null)
                {
                    // We can't match on null names so it can't be a match
                    return false;
                }

                if (types?.Length == 0)
                {
                    // We are only matching by name
                    return referenceName.Equals(name, StringComparison.OrdinalIgnoreCase);
                }

                var matches = from x in types
                    where x == type && referenceName.Equals(name, StringComparison.OrdinalIgnoreCase)
                    select x;

                return matches.Any();
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueGeneratorMatcher" /> class.
        /// </summary>
        /// <param name="expression">Identifies the possible parameter or property name regular expression the generator can match.</param>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        protected ValueGeneratorMatcher(Regex expression, params Type[] types)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            _matcher = (type, name, context) =>
            {
                if (name == null)
                {
                    // We can't match on null names with Regex so it can be a match
                    return false;
                }

                if (types?.Length == 0)
                {
                    // We are only matching by name
                    return expression.IsMatch(name);
                }

                var matches = from x in types
                    where x == type && expression.IsMatch(name)
                    select x;

                return matches.Any();
            };
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        protected override bool IsMatch(IBuildChain buildChain, Type type, string? referenceName)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            return _matcher(type, referenceName, buildChain);
        }
    }
}