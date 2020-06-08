namespace ModelBuilder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="DefaultParameterResolver" />
    ///     class is used to determine the ordered set of parameters that need to be created.
    /// </summary>
    public class DefaultParameterResolver : IParameterResolver
    {
        private static readonly ConcurrentDictionary<MethodBase, IList<ParameterInfo>> _globalParametersCache =
            new ConcurrentDictionary<MethodBase, IList<ParameterInfo>>();

        private readonly ConcurrentDictionary<MethodBase, IList<ParameterInfo>> _perInstanceParametersCache =
            new ConcurrentDictionary<MethodBase, IList<ParameterInfo>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultParameterResolver" /> class.
        /// </summary>
        /// <param name="cacheLevel">The cache level to use for resolved parameters.</param>
        public DefaultParameterResolver(CacheLevel cacheLevel)
        {
            CacheLevel = cacheLevel;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> parameter is <c>null</c>.</exception>
        public IEnumerable<ParameterInfo> GetOrderedParameters(IBuildConfiguration configuration, MethodBase method)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (CacheLevel == CacheLevel.Global)
            {
                return _globalParametersCache.GetOrAdd(method,
                    x => CalculateOrderedParameters(configuration, method).ToList());
            }

            if (CacheLevel == CacheLevel.PerInstance)
            {
                return _perInstanceParametersCache.GetOrAdd(method,
                    x => CalculateOrderedParameters(configuration, method).ToList());
            }

            return CalculateOrderedParameters(configuration, method);
        }

        private static IOrderedEnumerable<ParameterInfo> CalculateOrderedParameters(IBuildConfiguration configuration,
            MethodBase method)
        {
            return from x in method.GetParameters()
                orderby GetMaximumOrderPriority(configuration, x) descending
                select x;
        }

        private static int GetMaximumOrderPriority(IBuildConfiguration configuration, ParameterInfo parameter)
        {
            if (configuration.ExecuteOrderRules == null)
            {
                return 0;
            }

            var matchingRules = from x in configuration.ExecuteOrderRules
                where x.IsMatch(parameter)
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
        ///     Gets or sets whether identified parameters are cached.
        /// </summary>
        /// <returns>Returns the cache level to apply to parameters.</returns>
        public CacheLevel CacheLevel { get; set; }
    }
}