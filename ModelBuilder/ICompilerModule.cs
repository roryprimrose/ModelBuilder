namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="ICompilerModule" />
    ///     interface is used to define out to configure a <see cref="IBuildStrategyCompiler" /> using convention based
    ///     scanning.
    /// </summary>
    public interface ICompilerModule
    {
        /// <summary>
        ///     Configures the specified compiler.
        /// </summary>
        /// <param name="compiler">The compiler to configure.</param>
        void Configure(IBuildStrategyCompiler compiler);
    }
}