namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ModelExtensions
    {
        public static PropertyInfo GetProperty<T>(this T instance, Expression<Func<T, object?>> expression)
        {
            return expression.GetProperty();
        }
    }
}