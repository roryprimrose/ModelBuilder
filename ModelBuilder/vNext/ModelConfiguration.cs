namespace ModelBuilder.vNext
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     The <see cref="ModelConfiguration" /> class
    ///     accumulates ignore rules, type mappings and configuration modules for a single build, and
    ///     produces the configured instance.
    /// </summary>
    public sealed class ModelConfiguration
    {
        private readonly BuildConfiguration _configuration = new BuildConfiguration();

        /// <summary>
        ///     Creates a populated instance of <typeparamref name="T" /> using this configuration.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The created instance.</returns>
        public T Create<T>(params object?[]? args)
        {
            return Model.CreateWith<T>(_configuration, args);
        }

        /// <summary>
        ///     Adds an ignore rule for the specified member.
        /// </summary>
        /// <typeparam name="T">The type that declares the member.</typeparam>
        /// <param name="expression">An expression selecting the member to ignore.</param>
        /// <returns>The same configuration for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> does not select a member.</exception>
        public ModelConfiguration Ignoring<T>(Expression<Func<T, object?>> expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            _configuration.Ignore(typeof(T), GetMemberName(expression));

            return this;
        }

        /// <summary>
        ///     Adds a type mapping from a source type to a concrete target type.
        /// </summary>
        /// <typeparam name="TSource">The source type, typically an interface or abstract type.</typeparam>
        /// <typeparam name="TTarget">The concrete type to build in its place.</typeparam>
        /// <returns>The same configuration for chaining.</returns>
        public ModelConfiguration Mapping<TSource, TTarget>()
            where TTarget : TSource
        {
            _configuration.AddMapping<TSource, TTarget>();

            return this;
        }

        /// <summary>
        ///     Populates an existing instance of <typeparamref name="T" /> using this configuration.
        /// </summary>
        /// <typeparam name="T">The type to populate.</typeparam>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        public T Populate<T>(T instance)
        {
            return Model.PopulateWith(instance, _configuration);
        }

        /// <summary>
        ///     Applies a configuration module to this configuration.
        /// </summary>
        /// <typeparam name="TModule">The configuration module to apply.</typeparam>
        /// <returns>The same configuration for chaining.</returns>
        public ModelConfiguration UsingModule<TModule>()
            where TModule : IConfigurationModule, new()
        {
            new TModule().Configure(_configuration);

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
