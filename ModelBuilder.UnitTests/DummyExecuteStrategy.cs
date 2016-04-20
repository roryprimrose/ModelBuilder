﻿using System;
using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
    public class DummyExecuteStrategy : IExecuteStrategy<string>
    {
        public string CreateWith(params object[] args)
        {
            throw new NotImplementedException();
        }

        public string Populate(string instance)
        {
            throw new NotImplementedException();
        }

        public object CreateWith(Type type, params object[] args)
        {
            throw new NotImplementedException();
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public IConstructorResolver ConstructorResolver { get; set; } = new DefaultConstructorResolver();
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; } = new List<ExecuteOrderRule>();
        public ICollection<IgnoreRule> IgnoreRules { get; } = new List<IgnoreRule>();
        public ICollection<ITypeCreator> TypeCreators { get; } = new List<ITypeCreator>();
        public ICollection<IValueGenerator> ValueGenerators { get; } = new List<IValueGenerator>();
    }
}