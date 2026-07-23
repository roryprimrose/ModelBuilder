namespace ModelBuilder.IntegrationTests.Models
{
    using System;

    /// <summary>
    ///     A reusable <see cref="IConfigurationModule" /> bundling a mapping, an open generic mapping, an
    ///     ignore rule, an <c>IgnoringAny</c> predicate and an option override, so
    ///     <c>Model.UsingModule&lt;IntegrationTestModule&gt;()</c> can be verified end to end.
    /// </summary>
    public sealed class IntegrationTestModule : IConfigurationModule
    {
        /// <inheritdoc />
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.AddMapping<IEngine, DieselEngine>();

            configuration.AddMapping(typeof(IRepository<>), typeof(Repository<>));
            configuration.AddMapping<IRepository<Widget>, Repository<Widget>>();

            configuration.Ignore(typeof(Credentials), nameof(Credentials.Password));

            configuration.IgnoreAny(member => member.Name.EndsWith("Secret", StringComparison.Ordinal));

            configuration.SetOptions(x => x.MaxCount = 3);
        }
    }
}
