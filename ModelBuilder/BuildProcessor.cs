namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.BuildActions;

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
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
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
                throw new NotSupportedException();
            }

            return action.Build(executeStrategy, type);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
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
                throw new NotSupportedException();
            }

            return action.Build(executeStrategy, parameterInfo);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
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
                throw new NotSupportedException();
            }

            return action.Build(executeStrategy, propertyInfo);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
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

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var action = _actions.Where(x =>
                    x.IsMatch(executeStrategy.Configuration, executeStrategy.BuildChain, instance.GetType()).IsMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();

            if (action == null)
            {
                throw new NotSupportedException();
            }

            return action.Populate(executeStrategy, instance);
        }
    }
}