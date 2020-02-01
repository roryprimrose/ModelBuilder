﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DataSet
    {
        public static IEnumerable<object[]> GetParameters(Type type)
        {
            return from x in type.GetConstructors().Single().GetParameters()
                select new object[] {x};
        }

        public static IEnumerable<object[]> GetProperties(Type type)
        {
            return from x in type.GetProperties()
                select new object[] {x};
        }
    }
}