namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     The <see cref="CommonExtensions" />
    ///     class provides common extension methods.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        ///     Gets whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns><c>true</c> if the type is nullable; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public static bool IsNullable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (type.IsGenericType == false)
            {
                return false;
            }

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns a random item from the data set.
        /// </summary>
        /// <param name="source">The source data set.</param>
        /// <returns>A new data item.</returns>
        public static T Next<T>(this IReadOnlyList<T> source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            if (source.Count == 0)
            {
                return default!;
            }

            var generator = new RandomGenerator();

            var index = generator.NextValue(0, source.Count - 1);

            return source[index];
        }

        /// <summary>
        ///     Makes a change to the instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        public static T Set<T>(this T instance, Action<T> action)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            action = action ?? throw new ArgumentNullException(nameof(action));

            action(instance);

            return instance;
        }

        /// <summary>
        ///     Supports setting properties with inaccessible setters such as private or protected
        ///     Also limited support for setting of readonly auto-properties
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <typeparam name="TVALUE">The value to set the expresison function to.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="expressionFunc">The expresion function to set against the instance.</param>
        /// <param name="value"></param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expressionFunc" /> parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        ///     The <paramref name="expressionFunc" /> parameter is not supported - readonly
        ///     and complex properties are not supported.
        /// </exception>
        public static T Set<T, TVALUE>(this T instance, Expression<Func<T, TVALUE>> expressionFunc, TVALUE value)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));
            expressionFunc = expressionFunc ?? throw new ArgumentNullException(nameof(expressionFunc));

            var memberExpression = expressionFunc.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new NotSupportedException("Only properties and fields are supported");
            }

            var member = memberExpression.Member;

            var methodInfo = member as MethodInfo;
            if (methodInfo != null)
            {
                throw new NotSupportedException("Methods are not supported");
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                if (propertyInfo.GetSetMethod(true) != null)
                {
                    propertyInfo.SetValue(instance, value);
                    return instance;
                }

                var declaringType = propertyInfo.DeclaringType;
                if (declaringType == null)
                {
                    throw new NotSupportedException("Could not find declaring type");
                }

                var backingField = declaringType.GetField($"<{propertyInfo.Name}>k__BackingField",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (backingField != null
                    && backingField.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
                {
                    member = backingField;
                }
                else
                {
                    throw new NotSupportedException(
                        "Could not find a backing field - readonly properties are not supported");
                }
            }

            var fieldInfo = member as FieldInfo;
            if (fieldInfo == null)
            {
                throw new NotSupportedException("The member is not supported");
            }

            fieldInfo.SetValue(instance, value);

            return instance;
        }
    }
}