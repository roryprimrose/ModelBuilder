namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static class BuildConfigurationExtensions
    {


        /// <summary>
        ///     Adds a new post-build action to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="postBuildAction">The post-build action.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildAction" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, IPostBuildAction postBuildAction)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (postBuildAction == null)
            {
                throw new ArgumentNullException(nameof(postBuildAction));
            }

            compiler.PostBuildActions.Add(postBuildAction);

            return compiler;
        }

        /// <summary>
        ///     Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, ExecuteOrderRule rule)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, IgnoreRule rule)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            compiler.IgnoreRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, TypeMappingRule rule)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            compiler.TypeMappingRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new creation rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, CreationRule rule)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            compiler.CreationRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new type creator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="typeCreator">The type creator.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, ITypeCreator typeCreator)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (typeCreator == null)
            {
                throw new ArgumentNullException(nameof(typeCreator));
            }

            compiler.TypeCreators.Add(typeCreator);

            return compiler;
        }

        /// <summary>
        ///     Adds a new value generator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="valueGenerator">The value generator.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, IValueGenerator valueGenerator)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (valueGenerator == null)
            {
                throw new ArgumentNullException(nameof(valueGenerator));
            }

            compiler.ValueGenerators.Add(valueGenerator);

            return compiler;
        }

        /// <summary>
        ///     Adds configuration provided by the specified compiler module.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="module">The module.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="module" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration compiler, IConfigurationModule module)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            module.Configure(compiler);

            return compiler;
        }

        /// <summary>
        ///     Adds a configuration of a compiler module to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of compiler module to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCompilerModule<T>(this IBuildConfiguration compiler)
            where T : IConfigurationModule, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var module = new T();

            module.Configure(compiler);

            return compiler;
        }

        /// <summary>
        ///     Adds a new creation rule to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCreationRule<T>(this IBuildConfiguration compiler)
            where T : CreationRule, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new T();

            compiler.CreationRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new creation rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCreationRule<T>(
            this IBuildConfiguration compiler,
            Expression<Func<T, object>> expression,
            int priority,
            object value)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new CreationRule(targetType, property.Name, priority, value);

            compiler.CreationRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new execute order rule to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddExecuteOrderRule<T>(this IBuildConfiguration compiler)
            where T : ExecuteOrderRule, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new T();

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddExecuteOrderRule<T>(
            this IBuildConfiguration compiler,
            Expression<Func<T, object>> expression,
            int priority)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new ExecuteOrderRule(targetType, property.PropertyType, property.Name, priority);

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new ignore rule to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddIgnoreRule<T>(this IBuildConfiguration compiler)
            where T : IgnoreRule, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new T();

            compiler.IgnoreRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildConfiguration AddIgnoreRule<T>(
            this IBuildConfiguration compiler,
            Expression<Func<T, object>> expression)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new IgnoreRule(targetType, property.Name);

            compiler.IgnoreRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new post-build action to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddPostBuildAction<T>(this IBuildConfiguration compiler)
            where T : IPostBuildAction, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var postBuildAction = new T();

            compiler.PostBuildActions.Add(postBuildAction);

            return compiler;
        }

        /// <summary>
        ///     Adds a new type creator to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of type creator to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeCreator<T>(this IBuildConfiguration compiler)
            where T : ITypeCreator, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var creator = new T();

            compiler.TypeCreators.Add(creator);

            return compiler;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeMappingRule<T>(this IBuildConfiguration compiler)
            where T : TypeMappingRule, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new T();

            compiler.TypeMappingRules.Add(rule);

            return compiler;
        }

        /// <summary>
        ///     Adds a new value generator to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of value generator to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddValueGenerator<T>(this IBuildConfiguration compiler)
            where T : IValueGenerator, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var generator = new T();

            compiler.ValueGenerators.Add(generator);

            return compiler;
        }

        /// <summary>
        ///     Removes creation rules from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveCreationRule<T>(this IBuildConfiguration compiler)
            where T : CreationRule
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.CreationRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.CreationRules.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes execute order rules from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveExecuteOrderRule<T>(this IBuildConfiguration compiler)
            where T : ExecuteOrderRule
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.ExecuteOrderRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.ExecuteOrderRules.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes ignore rules from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveIgnoreRule<T>(this IBuildConfiguration compiler)
            where T : IgnoreRule
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.IgnoreRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.IgnoreRules.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes post-build actions from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemovePostBuildAction<T>(this IBuildConfiguration compiler)
            where T : IPostBuildAction
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.PostBuildActions.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.PostBuildActions.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes type creators from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of type creator to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeCreator<T>(this IBuildConfiguration compiler)
            where T : ITypeCreator
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.TypeCreators.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.TypeCreators.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes type mapping rules from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeMappingRule<T>(this IBuildConfiguration compiler)
            where T : TypeMappingRule
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.TypeMappingRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.TypeMappingRules.Remove(rule);
            }

            return compiler;
        }

        /// <summary>
        ///     Removes value generators from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value generator to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveValueGenerator<T>(this IBuildConfiguration compiler)
            where T : IValueGenerator
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var itemsToRemove = compiler.ValueGenerators.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                compiler.ValueGenerators.Remove(rule);
            }

            return compiler;
        }



    }
}