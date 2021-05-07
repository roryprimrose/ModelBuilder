namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class PublicPickedItem : IPublicOverInternal
    {
        public string First { get; set; } = string.Empty;
        public bool Second { get; set; }
        public Uri Third { get; set; } = new Uri("about:blank");
    }
}