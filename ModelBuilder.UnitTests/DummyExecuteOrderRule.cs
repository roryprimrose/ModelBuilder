﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using ModelBuilder.ExecuteOrderRules;

    public class DummyExecuteOrderRule : IExecuteOrderRule
    {
        public bool IsMatch(ParameterInfo parameter)
        {
            return false;
        }

        public bool IsMatch(PropertyInfo property)
        {
            return false;
        }

        public int Priority { get; } = 100;

        public Guid Value { get; set; }
    }
}