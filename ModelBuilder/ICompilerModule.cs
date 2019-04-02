namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="ICompilerModule" />
    ///     interface is used to configure a <see cref="IBuildStrategyCompiler" />.
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