namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IConfigurationModule" /> interface
    ///     defines a reusable unit of build configuration that registers type mappings and ignore
    ///     rules.
    /// </summary>
    public interface IConfigurationModule
    {
        /// <summary>
        ///     Applies this module's configuration to the supplied configuration.
        /// </summary>
        /// <param name="configuration">The configuration to populate.</param>
        void Configure(IBuildConfiguration configuration);
    }
}
