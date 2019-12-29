namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.UnitTests.Models;

    public class DummyExecuteOrderRule : ExecuteOrderRule
    {
        public DummyExecuteOrderRule() : base(
            typeof(Person),
            typeof(string),
            "SomeRandomPropertyWhichDoesNotExist",
            Environment.TickCount)
        {
        }
    }
}