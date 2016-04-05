using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides extension methods for the <see cref="ITypeCreator"/> interface.
    /// </summary>
    public static class TypeCreatorExtensions
    {
        /// <summary>
        /// Generates a new value of the specified type.
        /// </summary>
        /// <param name="creator">The creator that will create a value.</param>
        /// <param name="type">The type of value to create.</param>
        /// <returns>A new value of the type.</returns>
        public static object Create(this ITypeCreator creator, Type type)
        {
            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return creator.Create(type, null, null);
        }

        /// <summary>
        /// Returns whether the specified type is supported by this creator.
        /// </summary>
        /// <param name="creator">The creator that will create a value.</param>
        /// <param name="type">The type to evaulate.</param>
        /// <returns><c>true</c> if the type is supported; otherwise <c>false</c>.</returns>
        public static bool IsSupported(this ITypeCreator creator, Type type)
        {
            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return creator.IsSupported(type, null, null);
        }
    }
}