namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq.Expressions;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="IBuildStrategy" /> interface.
    /// </summary>
    public static class BuildStrategyExtensions
    {
        /// <summary>
        ///     Returns a new <see cref="IExecuteStrategy{T}" /> for the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to return.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <returns>A new execute strategy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        public static T UsingExecuteStrategy<T>(this IBuildStrategy buildStrategy) where T : IExecuteStrategy, new()
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            var buildLog = buildStrategy.GetBuildLog();

            if (buildLog == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.BuildStrategy_BuildLogRequired,
                    buildStrategy.GetType().FullName,
                    nameof(IBuildLog),
                    nameof(IExecuteStrategy<T>));

                throw new InvalidOperationException(message);
            }

            var executeStrategy = new T();

            executeStrategy.Initialize(buildStrategy, buildLog);

            return executeStrategy;
        }
    }
}