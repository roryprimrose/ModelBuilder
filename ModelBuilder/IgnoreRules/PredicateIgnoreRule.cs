namespace ModelBuilder.IgnoreRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="PredicateIgnoreRule" />
    ///     class is used to match the rule to a <see cref="PropertyInfo" /> using a predicate.
    /// </summary>
    public class PredicateIgnoreRule : IIgnoreRule
    {
        private readonly Predicate<PropertyInfo> _predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateIgnoreRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PredicateIgnoreRule(Predicate<PropertyInfo> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _predicate(propertyInfo);
        }
    }
}