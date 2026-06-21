// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.ExecuteOrderRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [Obsolete(DeprecationMessages.ExecuteOrder, true)]
    public interface IExecuteOrderRule
    {
    }

    [Obsolete(DeprecationMessages.ExecuteOrder, true)]
    [ExcludeFromCodeCoverage]
    public class ExpressionExecuteOrderRule<T>
    {
        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public ExpressionExecuteOrderRule(Expression<Func<T, object?>> expression, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();        [ExcludeFromCodeCoverage]
        public override string ToString() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ExecuteOrder, true)]
    [ExcludeFromCodeCoverage]
    public class ParameterPredicateExecuteOrderRule
    {
        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public ParameterPredicateExecuteOrderRule(Predicate<ParameterInfo> predicate, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();        [ExcludeFromCodeCoverage]
        public override string ToString() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ExecuteOrder, true)]
    [ExcludeFromCodeCoverage]
    public class PropertyPredicateExecuteOrderRule
    {
        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public PropertyPredicateExecuteOrderRule(Predicate<PropertyInfo> predicate, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();        [ExcludeFromCodeCoverage]
        public override string ToString() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ExecuteOrder, true)]
    [ExcludeFromCodeCoverage]
    public class RegexExecuteOrderRule
    {
        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public RegexExecuteOrderRule(Regex expression, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();        [ExcludeFromCodeCoverage]
        public override string ToString() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }
}
