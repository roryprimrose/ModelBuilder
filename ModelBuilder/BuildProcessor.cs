namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.BuildActions;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="IBuildProcessor" />
    ///     class is used to build types using a set of <see cref="IBuildAction" /> instances.
    /// </summary>
    public class BuildProcessor : IBuildProcessor
    {
        private readonly IList<IBuildAction> _actions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildProcessor" /> class.
        /// </summary>
        public BuildProcessor()
        {
            // Create the default set of actions
            _actions = new List<IBuildAction>
            {
                new CircularReferenceBuildAction(),
                new CreationRuleBuildAction(),
                new ValueGeneratorBuildAction()
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildProcessor" /> class.
        /// </summary>
        /// <param name="actions">The actions to use for the build processor.</param>
        public BuildProcessor(IEnumerable<IBuildAction> actions)
        {
            _actions = actions == null ? throw new ArgumentNullException(nameof(actions)) : actions.ToList();
        }

        /// <inheritdoc />
        public object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var action = _actions.Where(x =>
                    x.IsMatch(executeStrategy.Configuration, executeStrategy.BuildChain, type).IsMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();

            if (action == null)
            {
                throw CreateBuildException(type, null, executeStrategy.BuildChain, executeStrategy.Log);
            }

            return action.Build(executeStrategy, type);
        }

        /// <inheritdoc />
        public object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var action = _actions.Where(x =>
                    x.IsMatch(executeStrategy.Configuration, executeStrategy.BuildChain, parameterInfo).IsMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();

            if (action == null)
            {
                throw CreateBuildException(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                    executeStrategy.Log);
            }

            return action.Build(executeStrategy, parameterInfo);
        }

        /// <inheritdoc />
        public object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var action = _actions.Where(x =>
                    x.IsMatch(executeStrategy.Configuration, executeStrategy.BuildChain, propertyInfo).IsMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();

            if (action == null)
            {
                throw CreateBuildException(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                    executeStrategy.Log);
            }

            return action.Build(executeStrategy, propertyInfo);
        }

        /// <inheritdoc />
        public BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var results = from x in _actions
                orderby x.Priority descending
                select x.IsMatch(buildConfiguration, buildChain, type);

            var matchingActions = from x in results
                where x.IsMatch
                select x;

            return matchingActions.FirstOrDefault();
        }

        /// <inheritdoc />
        public BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var results = from x in _actions
                orderby x.Priority descending
                select x.IsMatch(buildConfiguration, buildChain, parameterInfo);

            var matchingActions = from x in results
                where x.IsMatch
                select x;

            return matchingActions.FirstOrDefault();
        }

        /// <inheritdoc />
        public BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var results = from x in _actions
                orderby x.Priority descending
                select x.IsMatch(buildConfiguration, buildChain, propertyInfo);

            var matchingActions = from x in results
                where x.IsMatch
                select x;

            return matchingActions.FirstOrDefault();
        }

        private Exception CreateBuildException(Type type, string referenceName, IBuildChain buildChain,
            IBuildLog buildLog)
        {
            string message;
            var context = buildChain.Last;

            if (string.IsNullOrWhiteSpace(referenceName) == false)
            {
                if (context != null)
                {
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NoMatchingBuildActionFoundWithNameAndContext,
                        type.FullName,
                        referenceName,
                        context.GetType().FullName);
                }
                else
                {
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NoMatchingBuildActionFoundWithName,
                        type.FullName,
                        referenceName);
                }
            }
            else
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.NoMatchingBuildActionFound,
                    type.FullName);
            }

            var ex = new NotSupportedException(message);

            buildLog.BuildFailure(ex);

            const string messageFormat =
                "Failed to create instance of type {0}, {1}: {2}{3}{3}At the time of the failure, the build log was:{3}{3}{4}";
            var logOutput = buildLog.Output;
            var failureMessage = string.Format(
                CultureInfo.CurrentCulture,
                messageFormat,
                type.FullName,
                ex.GetType().Name,
                ex.Message,
                Environment.NewLine,
                logOutput);

            return new BuildException(failureMessage, type, referenceName, context, logOutput, ex);
        }
    }
}