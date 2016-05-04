using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BuildStrategyCompilerExtensions"/>
    /// class provides extension methods for the <see cref="IBuildStrategyCompiler"/> interface.
    /// </summary>
    public static class BuildStrategyCompilerExtensions
    {
        /// <summary>
        /// Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule"/> parameter is <c>null</c>.</exception>
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
        /// Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule"/> parameter is <c>null</c>.</exception>
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
        /// Adds a new type creator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="typeCreator">The type creator.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreator"/> parameter is <c>null</c>.</exception>
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
        /// Adds a new value generator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="valueGenerator">The value generator.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator"/> parameter is <c>null</c>.</exception>
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
        /// Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="evaluator"/> parameter is null.</exception>
        public static IBuildStrategyCompiler AddExecuteOrderRule(this IBuildStrategyCompiler compiler,
            Func<Type, string, bool> evaluator, int priority)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new ExecuteOrderRule(evaluator, priority);

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        /// Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyExpression">The property name regular expression that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyExpression"/> parameters are both null.</exception>
        public static IBuildStrategyCompiler AddExecuteOrderRule(this IBuildStrategyCompiler compiler, Type targetType,
            Regex propertyExpression, int priority)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new ExecuteOrderRule(targetType, propertyExpression, priority);

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        /// Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyName"/> parameters are both null.</exception>
        public static IBuildStrategyCompiler AddExecuteOrderRule(this IBuildStrategyCompiler compiler, Type targetType,
            string propertyName, int priority)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new ExecuteOrderRule(targetType, propertyName, priority);

            compiler.ExecuteOrderRules.Add(rule);

            return compiler;
        }

        /// <summary>
        /// Adds a new execute order rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type."
            )]
        public static IBuildStrategyCompiler AddExecutionOrderRule<T>(this IBuildStrategyCompiler compiler)
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
        /// Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type."
            )]
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
        /// Adds a new ignore rule to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> parameter is null, only contains whitespace or is empty.</exception>
        public static IBuildStrategyCompiler AddIgnoreRule(this IBuildStrategyCompiler compiler, Type targetType,
            string propertyName)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            var rule = new IgnoreRule(targetType, propertyName);

            compiler.IgnoreRules.Add(rule);

            return compiler;
        }

        /// <summary>
        /// Adds a new type creator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type."
            )]
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
        /// Adds a new value generator to the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type."
            )]
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
        /// Sets the constructor resolver on the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type."
            )]
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
        /// Sets the constructor resolver on the compiler.
        /// </summary>
        /// <param name="compiler">The compiler.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="compiler"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resolver"/> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler SetConstructorResolver(this IBuildStrategyCompiler compiler,
            IConstructorResolver resolver)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler));
            }

            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            compiler.ConstructorResolver = resolver;

            return compiler;
        }
    }
}