namespace ModelBuilder.UnitTests
{
    using System;

    public class DummyExecuteOrderRule : ExecuteOrderRule
    {
        public DummyExecuteOrderRule() : base(typeof(Person), typeof(string), "SomeRandomPropertyWhichDoesNotExist", Environment.TickCount)
        {
        }
    }
}