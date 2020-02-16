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
                new ValueGeneratorBuildAction(),
                new TypeCreatorBuildAction()
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

            var match = GetMatch(BuildRequirement.Create, x => x.GetBuildCapability(executeStrategy.Configuration,
                executeStrategy.BuildChain,
                type));

            if (match == null)
            {
                throw new NotSupportedException();
            }

            return match.Action.Build(executeStrategy, type, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var match = GetMatch(BuildRequirement.Create, x => x.GetBuildCapability(executeStrategy.Configuration,
                executeStrategy.BuildChain, parameterInfo));

            if (match == null)
            {
                throw new NotSupportedException();
            }

            return match.Action.Build(executeStrategy, parameterInfo, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var match = GetMatch(BuildRequirement.Create, x => x.GetBuildCapability(executeStrategy.Configuration,
                executeStrategy.BuildChain, propertyInfo));

            if (match == null)
            {
                throw new NotSupportedException();
            }

            return match.Action.Build(executeStrategy, propertyInfo, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            BuildRequirement buildRequirement, Type type)
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

            var match = GetMatch(buildRequirement, x => x.GetBuildCapability(buildConfiguration,
                buildChain,
                type));

            if (match == null)
            {
                return null;
            }

            return match.Capability;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            BuildRequirement buildRequirement,
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

            var match = GetMatch(buildRequirement, x => x.GetBuildCapability(buildConfiguration,
                buildChain,
                parameterInfo));

            if (match == null)
            {
                return null;
            }

            return match.Capability;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            BuildRequirement buildRequirement,
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

            var match = GetMatch(buildRequirement, x => x.GetBuildCapability(buildConfiguration,
                buildChain,
                propertyInfo));

            if (match == null)
            {
                return null;
            }

            return match.Capability;
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

            var match = GetMatch(BuildRequirement.Populate, x => x.GetBuildCapability(executeStrategy.Configuration,
                executeStrategy.BuildChain, instance.GetType()));

            if (match == null)
            {
                throw new NotSupportedException();
            }

            return match.Action.Populate(executeStrategy, instance);
        }

        private Match GetMatch(BuildRequirement buildRequirement, Func<IBuildAction, BuildCapability> evaluator)
        {
            var capabilities = from x in _actions
                orderby x.Priority descending
                select new Match
                {
                    Action = x,
                    Capability = evaluator(x)
                };

            var matches = from x in capabilities
                where x.Capability != null
                      && (x.Capability.SupportsCreate && buildRequirement == BuildRequirement.Create
                          || x.Capability.SupportsPopulate && buildRequirement == BuildRequirement.Populate)
                select x;

            return matches.FirstOrDefault();
        }

        private class Match
        {
            public IBuildAction Action { get; set; }
            public BuildCapability Capability { get; set; }
        }
    }
}