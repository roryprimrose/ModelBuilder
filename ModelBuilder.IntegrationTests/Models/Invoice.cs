namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A base class whose settable properties should populate on a derived type just like its own.
    /// </summary>
    public class AuditableEntity
    {
        public System.Guid Id { get; set; }

        public System.DateTime CreatedOn { get; set; }
    }

    /// <summary>
    ///     A derived type mixing its own members with inherited ones, to verify the generator populates
    ///     both.
    /// </summary>
    public class Invoice : AuditableEntity
    {
        public string Reference { get; set; } = string.Empty;

        public decimal Total { get; set; }
    }
}
