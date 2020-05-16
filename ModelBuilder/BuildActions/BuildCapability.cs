namespace ModelBuilder.BuildActions
{
    using System;

    /// <summary>
    ///     The <see cref="BuildCapability" />
    ///     class is used to identify how <see cref="IExecuteStrategy" /> should operate when using
    ///     <see cref="IBuildProcessor" />.
    /// </summary>
    public class BuildCapability
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCapability"/> class.
        /// </summary>
        /// <param name="implementedByType">The type provide provides the implementation of the capability.</param>
        public BuildCapability(Type implementedByType)
        {
            ImplementedByType = implementedByType ?? throw new ArgumentNullException(nameof(implementedByType));
        }

        /// <summary>
        ///     Gets or sets whether attempts to create the instance requires a constructor to be automatically resolved.
        /// </summary>
        /// <remarks>
        ///     When set to <c>true</c>, detection of the appropriate constructor and creation of constructor parameters will
        ///     be required.
        /// </remarks>
        public bool AutoDetectConstructor { get; set; }

        /// <summary>
        ///     Gets or sets whether properties on the created value should be automatically populated.
        /// </summary>
        public bool AutoPopulate { get; set; }

        /// <summary>
        ///     Gets or sets the type that will be used to run a create or populate process.
        /// </summary>
        public Type ImplementedByType { get; }

        /// <summary>
        ///     Gets or sets whether there the <see cref="IBuildAction" /> supports the requested scenario.
        /// </summary>
        public bool SupportsCreate { get; set; }

        /// <summary>
        ///     Gets or sets whether a build action supports populating the created value with its own logic.
        /// </summary>
        public bool SupportsPopulate { get; set; }
    }
}