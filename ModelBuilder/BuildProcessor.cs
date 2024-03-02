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
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement, Type type)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var match = GetCapability(executeStrategy, buildRequirement, x => x.GetBuildCapability(
                executeStrategy.Configuration, executeStrategy.BuildChain,
                type), type, null);

            return match;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement,
            ParameterInfo parameterInfo)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            var match = GetCapability(executeStrategy, buildRequirement, x => x.GetBuildCapability(
                executeStrategy.Configuration, executeStrategy.BuildChain,
                parameterInfo), parameterInfo.ParameterType, parameterInfo.Name);

            return match;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement,
            PropertyInfo propertyInfo)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            var match = GetCapability(executeStrategy, buildRequirement, x => x.GetBuildCapability(
                executeStrategy.Configuration, executeStrategy.BuildChain,
                propertyInfo), propertyInfo.PropertyType, propertyInfo.Name);

            return match;
        }

        private IBuildCapability GetCapability(IExecuteStrategy executeStrategy, BuildRequirement buildRequirement,
            Func<IBuildAction, IBuildCapability?> evaluator, Type targetType, string? referenceName)
        {
            var capabilities = from x in _actions
                orderby x.Priority descending
                select evaluator(x);

            var matches = from x in capabilities
                where x != null
                      && ((x.SupportsCreate && buildRequirement == BuildRequirement.Create)
                          || (x.SupportsPopulate && buildRequirement == BuildRequirement.Populate))
                select x;

            var capability = matches.FirstOrDefault();

            if (capability == null)
            {
                var message = $"Failed to identify build capabilities for {targetType.FullName}.";

                throw new BuildException(message, targetType, referenceName, null, executeStrategy.Log.Output);
            }

            return capability;
        }
    }
}