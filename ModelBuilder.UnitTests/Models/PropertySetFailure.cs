namespace ModelBuilder.UnitTests.Models
{
    using System;

    internal class PropertySetFailure
    {
        public string Name { get => "Name"; set => throw new NotImplementedException(); }
    }
}