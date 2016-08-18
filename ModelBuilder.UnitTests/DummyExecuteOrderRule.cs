namespace ModelBuilder.UnitTests
{
    using System;

    public class DummyExecuteOrderRule : ExecuteOrderRule
    {
        public DummyExecuteOrderRule() : base(typeof(string), "SomeRandomPropertyWhichDoesNotExist", Environment.TickCount)
        {
        }
    }
}