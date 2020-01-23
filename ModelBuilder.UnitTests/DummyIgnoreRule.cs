﻿namespace ModelBuilder.UnitTests
{
    using System.Reflection;
    using ModelBuilder.IgnoreRules;

    public class DummyIgnoreRule : IIgnoreRule
    {
        public bool IsMatch(PropertyInfo property)
        {
            return false;
        }
    }
}