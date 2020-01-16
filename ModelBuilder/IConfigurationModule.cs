namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IConfigurationModule" />
    ///     interface is used to configure a <see cref="IBuildConfiguration" />.
    /// </summary>
    public interface IConfigurationModule
    {
        /// <summary>
        ///     Configures the specified build configuration.
        /// </summary>
        /// <param name="configuration">The build configuration to update.</param>
        void Configure(IBuildConfiguration configuration);
    }
}