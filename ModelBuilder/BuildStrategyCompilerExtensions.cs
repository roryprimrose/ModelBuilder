namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

#if NET45
    using System.Collections.Generic;
    using System.Reflection;
#endif

    /// <summary>
    ///     The <see cref="BuildStrategyCompilerExtensions" />
    ///     class provides extension methods for the <see cref="IBuildStrategyCompiler" /> interface.
    /// </summary>
    public static class BuildStrategyCompilerExtensions
    {
        /// <summary>
        ///     Adds a new post-build action to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="postBuildAction">The post-build action.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildAction" /> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, IPostBuildAction postBuildAction)
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
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, ExecuteOrderRule rule)
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
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, IgnoreRule rule)
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
        ///     Adds a new creation rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, CreationRule rule)
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
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, ITypeCreator typeCreator)
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
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, IValueGenerator valueGenerator)
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
        public static IBuildStrategyCompiler Add(this IBuildStrategyCompiler compiler, ICompilerModule module)
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
        ///     Adds a new creation rule to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddCreationRule<T>(this IBuildStrategyCompiler compiler)
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
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddCreationRule<T>(
            this IBuildStrategyCompiler compiler,
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddExecuteOrderRule<T>(this IBuildStrategyCompiler compiler)
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
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddExecuteOrderRule<T>(
            this IBuildStrategyCompiler compiler,
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddIgnoreRule<T>(this IBuildStrategyCompiler compiler)
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
        ///     Adds a configuration of a compiler module to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of compiler module to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddCompilerModule<T>(this IBuildStrategyCompiler compiler)
            where T : ICompilerModule, new()
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
        ///     Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildStrategyCompiler AddIgnoreRule<T>(
            this IBuildStrategyCompiler compiler,
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddPostBuildAction<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddTypeCreator<T>(this IBuildStrategyCompiler compiler)
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
        ///     Adds a new value generator to the compiler.
        /// </summary>
        /// <typeparam name="T">The type of value generator to add.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler AddValueGenerator<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemoveCreationRule<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemoveExecuteOrderRule<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemoveIgnoreRule<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemovePostBuildAction<T>(this IBuildStrategyCompiler compiler)
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
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemoveTypeCreator<T>(this IBuildStrategyCompiler compiler)
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
        ///     Removes value generators from the compiler that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value generator to remove.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler RemoveValueGenerator<T>(this IBuildStrategyCompiler compiler)
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

#if NET45
/// <summary>
///     Scans available assemblies for <see cref="ICompilerModule" /> types that can configure the specified compiler.
/// </summary>
/// <param name="compiler">The compiler to configure.</param>
/// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        public static void ScanModules(this IBuildStrategyCompiler compiler)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var types = GetAvailableAssemblies().SelectMany(x => x.GetLoadableTypes());
            var modules = from x in types
                where typeof(ICompilerModule).IsAssignableFrom(x)
                      && (x.TypeIsAbstract() == false)
                      && (x.TypeIsInterface() == false)
                select (ICompilerModule) Activator.CreateInstance(x);

            foreach (var module in modules)
            {
                module.Configure(compiler);
            }
        }
        private static Assembly[] GetAvailableAssemblies()
        {
            // NOTE: This is written this way so that a future version can use compiler
            // constants to provide a different implementation for netstandard
            var appDomain = AppDomain.CurrentDomain;

            appDomain.ReflectionOnlyAssemblyResolve += (sender, args) => Assembly.ReflectionOnlyLoad(args.Name);

            return appDomain.GetAssemblies();
        }

        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            IEnumerable<Type> types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(x => x != null);
            }

            return types;
        }
#endif

        /// <summary>
        ///     Sets the constructor resolver on the compiler.
        /// </summary>
        /// <typeparam name="T">The type of constructor resolver to use.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler SetConstructorResolver<T>(this IBuildStrategyCompiler compiler)
            where T : IConstructorResolver, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var resolver = new T();

            compiler.ConstructorResolver = resolver;

            return compiler;
        }

        /// <summary>
        ///     Sets the constructor resolver on the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resolver" /> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler SetConstructorResolver(
            this IBuildStrategyCompiler compiler,
            IConstructorResolver resolver)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            compiler.ConstructorResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            return compiler;
        }

        /// <summary>
        ///     Sets the property resolver on the compiler.
        /// </summary>
        /// <typeparam name="T">The type of property resolver to use.</typeparam>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildStrategyCompiler SetPropertyResolver<T>(this IBuildStrategyCompiler compiler)
            where T : IPropertyResolver, new()
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var resolver = new T();

            compiler.PropertyResolver = resolver;

            return compiler;
        }

        /// <summary>
        ///     Sets the property resolver on the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resolver" /> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler SetPropertyResolver(
            this IBuildStrategyCompiler compiler,
            IPropertyResolver resolver)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            compiler.PropertyResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

            return compiler;
        }
    }
}