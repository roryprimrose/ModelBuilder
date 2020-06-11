namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ExecuteStrategyExtensions" />
    ///     class is used to provide extension methods for the <see cref="IExecuteStrategy" /> interface.
    /// </summary>
    public static class ExecuteStrategyExtensions
    {
        /// <summary>
        ///     Writes the log entry using the specified action after the execute strategy is invoked.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy to invoke.</param>
        /// <param name="action">The logging action to call.</param>
        /// <returns>The execute strategy to invoke.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        public static IExecuteStrategy WriteLog(this IExecuteStrategy executeStrategy, Action<string> action)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return new LoggingExecuteStrategy<IExecuteStrategy>(executeStrategy, action);
        }

        /// <summary>
        ///     Writes the log entry using the specified action after the execute strategy is invoked.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy to invoke.</param>
        /// <param name="action">The logging action to call.</param>
        /// <typeparam name="T">The type of instance to create and populate.</typeparam>
        /// <returns>The execute strategy to invoke.</returns>
        public static IExecuteStrategy<T> WriteLog<T>(this IExecuteStrategy<T> executeStrategy, Action<string> action)
            where T : notnull
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return new LoggingGenericExecuteStrategy<T>(executeStrategy, action);
        }

        private class LoggingExecuteStrategy<T> : IExecuteStrategy where T : IExecuteStrategy
        {
            public LoggingExecuteStrategy(T child, Action<string> action)
            {
                Child = child;
                LogAction = action;
            }

            public object Create(Type type, params object?[]? args)
            {
                var value = Child.Create(type, args);

                LogAction(Log.Output);

                return value;
            }

            public object?[]? CreateParameters(MethodBase method)
            {
                return Child.CreateParameters(method);
            }

            public void Initialize(IBuildConfiguration configuration)
            {
                Child.Initialize(configuration);
            }

            public object Populate(object instance)
            {
                var value = Child.Populate(instance);

                LogAction(Log.Output);

                return value;
            }

            public IBuildChain BuildChain => Child.BuildChain;
            public IBuildConfiguration Configuration => Child.Configuration;
            public IBuildLog Log => Child.Log;

            protected T Child { get; }
            protected Action<string> LogAction { get; }
        }

        private class LoggingGenericExecuteStrategy<T> : LoggingExecuteStrategy<IExecuteStrategy<T>>,
            IExecuteStrategy<T> where T : notnull
        {
            public LoggingGenericExecuteStrategy(IExecuteStrategy<T> child, Action<string> action) : base(child, action)
            {
            }

            public T Create(params object?[]? args)
            {
                var value = Child.Create(args);

                LogAction(Log.Output);

                return value;
            }

            public T Populate(T instance)
            {
                var value = Child.Populate(instance);

                LogAction(Log.Output);

                return value;
            }
        }
    }
}