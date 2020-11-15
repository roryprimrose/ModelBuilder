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
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var rule = GetMatchingRule(x => x.IsMatch(type), executeStrategy.Configuration);

            return Build(rule, type, null, executeStrategy.BuildChain,
                () => rule?.Create(executeStrategy, type),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            var rule = GetMatchingRule(x => x.IsMatch(parameterInfo), executeStrategy.Configuration);

            return Build(rule, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => rule?.Create(executeStrategy, parameterInfo),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            var rule = GetMatchingRule(x => x.IsMatch(propertyInfo), executeStrategy.Configuration);

            return Build(rule, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => rule?.Create(executeStrategy, propertyInfo),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            Type type)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            type = type ?? throw new ArgumentNullException(nameof(type));

            return GetBuildCapability(x => x.IsMatch(type), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            return GetBuildCapability(x => x.IsMatch(parameterInfo), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            return GetBuildCapability(x => x.IsMatch(propertyInfo), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotSupportedException();
        }

        private static object? Build(ICreationRule? rule, Type typeToBuild, string? referenceName,
            IBuildChain buildChain,
            Func<object?> createAction, IBuildLog buildLog)
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

        private static IBuildCapability? GetBuildCapability(Func<ICreationRule, bool> isMatch,
            IBuildConfiguration buildConfiguration)
        {
            var rule = GetMatchingRule(isMatch, buildConfiguration);

            if (rule == null)
            {
                return null;
            }

            return new BuildCapability(rule);
        }

        private static ICreationRule? GetMatchingRule(Func<ICreationRule, bool> isMatch,
            IBuildConfiguration buildConfiguration)
        {
            return buildConfiguration.CreationRules.Where(isMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 5000;
    }
}