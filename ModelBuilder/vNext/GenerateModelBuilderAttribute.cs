namespace ModelBuilder.vNext
{
    using System;

    /// <summary>
    ///     The <see cref="GenerateModelBuilderAttribute" /> class
    ///     marks a type as an explicit build root so the generator emits a builder for it even when it
    ///     is never named directly in a <c>Model.Create&lt;T&gt;()</c> call.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Use the assembly-level form <c>[assembly: GenerateModelBuilder(typeof(X))]</c> from a test
    ///         assembly to opt a production type into generation without shipping the attribute in the
    ///         production binary. Use the type-level form only for types that exist solely in the test
    ///         assembly.
    ///     </para>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class GenerateModelBuilderAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GenerateModelBuilderAttribute" /> class for
        ///     the type the attribute is applied to.
        /// </summary>
        public GenerateModelBuilderAttribute()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenerateModelBuilderAttribute" /> class for the
        ///     specified target type.
        /// </summary>
        /// <param name="targetType">The type to emit a builder for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        public GenerateModelBuilderAttribute(Type targetType)
        {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        /// <summary>
        ///     Gets the target type to emit a builder for.
        /// </summary>
        /// <returns>
        ///     The target type for the assembly-level form, or <c>null</c> when the attribute is applied
        ///     directly to a type.
        /// </returns>
        public Type? TargetType { get; }
    }
}
