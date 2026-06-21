namespace ModelBuilder
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     The <see cref="ModelConfiguration" /> class
    ///     accumulates ignore rules, type mappings and configuration modules for a single build, and
    ///     produces the configured instance.
    /// </summary>
    internal sealed class ModelConfiguration : IModelConfiguration
    {
        private readonly BuildConfiguration _configuration = new BuildConfiguration();
        private Action<string>? _logSink;

        /// <inheritdoc />
        public T Create<T>()
        {
            if (_logSink == null)
            {
                return Model.CreateWith<T>(_configuration);
            }

            var log = new BuildLog();

            try
            {
                return Model.CreateWith<T>(_configuration, log);
            }
            finally
            {
                _logSink(log.Render());
            }
        }

        /// <inheritdoc />
        public IModelConfiguration Ignoring<T>(Expression<Func<T, object?>> expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            _configuration.Ignore(typeof(T), GetMemberName(expression));

            return this;
        }

        /// <inheritdoc />
        public IModelConfiguration Mapping<TSource, TTarget>()
            where TTarget : TSource
        {
            _configuration.AddMapping<TSource, TTarget>();

            return this;
        }

        /// <inheritdoc />
        public Construction<T> Construct<T>()
        {
            return new Construction<T>(_configuration, _logSink);
        }

        /// <inheritdoc />
        public IModelConfiguration AddValueSource<T>(IValueSource<T> source)
        {
            _configuration.AddValueSource(source);

            return this;
        }

        /// <inheritdoc />
        public IModelConfiguration AddValueSource<T>(IValueSource<T> source, params string[] names)
        {
            _configuration.AddValueSource(source, names);

            return this;
        }

        /// <inheritdoc />
        public T Populate<T>(T instance)
        {
            if (_logSink == null)
            {
                return Model.PopulateWith(instance, _configuration);
            }

            var log = new BuildLog();

            try
            {
                return Model.PopulateWith(instance, _configuration, log);
            }
            finally
            {
                _logSink(log.Render());
            }
        }

        /// <inheritdoc />
        public IModelConfiguration UsingModule<TModule>()
            where TModule : IConfigurationModule, new()
        {
            new TModule().Configure(_configuration);

            return this;
        }

        /// <inheritdoc />
        public IModelConfiguration WriteLog(Action<string> sink)
        {
            _logSink = sink ?? throw new ArgumentNullException(nameof(sink));

            return this;
        }

        private static string GetMemberName<T>(Expression<Func<T, object?>> expression)
        {
            var body = expression.Body;

            if (body is UnaryExpression unary)
            {
                body = unary.Operand;
            }

            if (body is MemberExpression member)
            {
                return member.Member.Name;
            }

            throw new ArgumentException(
                "The expression must select a member, for example 'x => x.FirstName'.",
                nameof(expression));
        }
    }
}
