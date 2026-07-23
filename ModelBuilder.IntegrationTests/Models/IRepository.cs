namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     An open generic abstraction with no default concrete type, used to verify
    ///     <c>Model.Mapping(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;))</c> resolves any closed
    ///     shape of it (paired with an explicit closed <c>Mapping&lt;,&gt;()</c> declaration) to a fully
    ///     built concrete instance.
    /// </summary>
    public interface IRepository<T>
    {
        T? Item { get; }
    }
}
