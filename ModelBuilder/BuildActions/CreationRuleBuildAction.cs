namespace ModelBuilder.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.CreationRules;

    /// <summary>
    ///     The <see cref="CreationRuleBuildAction" />
    ///     class is used to build a value from a matching <see cref="ICreationRule" />.
    /// </summary>
    public class CreationRuleBuildAction : IBuildAction
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(Type type, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var rule = GetMatchingRule(type, null, executeStrategy.Configuration);

            return rule?.Create(type, null, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var rule = GetMatchingRule(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.Configuration);

            return rule?.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var rule = GetMatchingRule(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.Configuration);

            return rule?.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(Type type, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var rule = GetMatchingRule(type, null, buildConfiguration);

            if (rule == null)
            {
                return false;
            }

            return rule.IsMatch(type, null);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var rule = GetMatchingRule(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration);

            if (rule == null)
            {
                return false;
            }

            return rule.IsMatch(parameterInfo.ParameterType, parameterInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var rule = GetMatchingRule(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration);

            if (rule == null)
            {
                return false;
            }

            return rule.IsMatch(propertyInfo.PropertyType, propertyInfo.Name);
        }

        private ICreationRule GetMatchingRule(Type type, string referenceName, IBuildConfiguration buildConfiguration)
        {
            return buildConfiguration.CreationRules?.Where(x => x.IsMatch(type, referenceName))
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 5000;
    }
}