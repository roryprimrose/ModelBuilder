namespace ModelBuilder.UnitTests.Models
{
    using System;

    public interface IPublicOverInternal
    {
        public string First { get; }
        public bool Second { get; }
        public Uri Third { get; }
    }
}