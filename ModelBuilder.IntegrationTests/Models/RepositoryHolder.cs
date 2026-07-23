namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A class with a closed, generic interface-typed member that has no default concrete type,
    ///     requiring an open generic mapping registration (plus a closed shape declaration) for the build
    ///     to populate it.
    /// </summary>
    public class RepositoryHolder
    {
        public IRepository<Widget>? Repository { get; set; }
    }
}
