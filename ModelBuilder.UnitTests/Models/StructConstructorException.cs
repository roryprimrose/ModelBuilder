namespace ModelBuilder.UnitTests.Models
{
    using System;

    internal struct StructConstructorException
    {
        public StructConstructorException()
        {
            throw new InvalidOperationException("This is an exception from the constructor.");
        }
    }
}