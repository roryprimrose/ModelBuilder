﻿namespace ModelBuilder
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
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            action = action ?? throw new ArgumentNullException(nameof(action));

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
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            action = action ?? throw new ArgumentNullException(nameof(action));

            return new LoggingGenericExecuteStrategy<T>(executeStrategy, action);
        }

        private class LoggingExecuteStrategy<T> : IExecuteStrategy where T : IExecuteStrategy
        {
            public LoggingExecuteStrategy(T child, Action<string> action)
            {
                Child = child;
                LogAction = action;

                // Enable logging on the child execution strategy
                Child.Log.IsEnabled = true;
            }

            public object Create(Type type, params object?[]? args)
            {
                try
                {
                    var value = Child.Create(type, args);

                    return value;
                }
                finally
                {
                    LogAction(Log.Output);
                }
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
                try
                {
                    var value = Child.Populate(instance);

                    return value;
                }
                finally
                {
                    LogAction(Log.Output);
                }
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
                try
                {
                    var value = Child.Create(args);

                    return value;
                }
                finally
                {
                    LogAction(Log.Output);
                }
            }

            public T Populate(T instance)
            {
                try
                {
                    var value = Child.Populate(instance);

                    return value;
                }
                finally
                {
                    LogAction(Log.Output);
                }
            }
        }
    }
}