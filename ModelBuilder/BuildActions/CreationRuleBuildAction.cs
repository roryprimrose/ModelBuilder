namespace ModelBuilder.BuildActions
{
    using System;
    using System.Globalization;
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
        public object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments)
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

            return Build(rule, type, null, executeStrategy.BuildChain,
                () => rule?.Create(type, null, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
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

            return Build(rule, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => rule?.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
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

            return Build(rule, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => rule?.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type)
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
        public bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo)
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
        public bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo)
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

        private object Build(ICreationRule rule, Type typeToBuild, string referenceName, IBuildChain buildChain,
            Func<object> createAction, IBuildLog buildLog)
        {
            if (rule == null)
            {
                return null;
            }

            var context = buildChain.Last;
            var ruleType = rule.GetType();

            buildLog.CreatingValue(typeToBuild, ruleType, context);

            try
            {
                return createAction();
            }
            catch (BuildException)
            {
                throw;
            }
            catch (Exception ex)
            {
                buildLog.BuildFailure(ex);

                const string messageFormat =
                    "Failed to create value for type {0} using creation rule {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                var output = buildLog.Output;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    messageFormat,
                    typeToBuild.FullName,
                    ruleType.FullName,
                    ex.GetType().Name,
                    ex.Message,
                    Environment.NewLine,
                    output);

                throw new BuildException(message, typeToBuild, referenceName, context, output, ex);
            }
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