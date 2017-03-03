namespace ModelBuilder.UnitTests
{
    using System;

    public class SelfReferrer
    {
        public Guid Id { get; set; }
        public SelfReferrer Self { get; set; }
    }
}