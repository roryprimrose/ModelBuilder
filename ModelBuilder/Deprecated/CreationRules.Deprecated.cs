// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.CreationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [Obsolete(DeprecationMessages.CreationRule, true)]
    public interface ICreationRule
    {
    }

    [Obsolete(DeprecationMessages.CreationRule, true)]
    [ExcludeFromCodeCoverage]
    public class ExpressionCreationRule<T>
    {
        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public ExpressionCreationRule(Expression<Func<T, object?>> expression, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public ExpressionCreationRule(Expression<Func<T, object?>> expression, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();        [ExcludeFromCodeCoverage]
        public override string ToString() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.CreationRule, true)]
    [ExcludeFromCodeCoverage]
    public class ParameterPredicateCreationRule
    {
        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public ParameterPredicateCreationRule(Predicate<ParameterInfo> predicate, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public ParameterPredicateCreationRule(Predicate<ParameterInfo> predicate, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.CreationRule, true)]
    [ExcludeFromCodeCoverage]
    public class PropertyPredicateCreationRule
    {
        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public PropertyPredicateCreationRule(Predicate<PropertyInfo> predicate, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public PropertyPredicateCreationRule(Predicate<PropertyInfo> predicate, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.CreationRule, true)]
    [ExcludeFromCodeCoverage]
    public class RegexCreationRule
    {
        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexCreationRule(Type targetType, Regex expression, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexCreationRule(Type targetType, Regex expression, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexCreationRule(Type targetType, string expression, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexCreationRule(Type targetType, string expression, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.CreationRule, true)]
    [ExcludeFromCodeCoverage]
    public class TypePredicateCreationRule
    {
        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public TypePredicateCreationRule(Predicate<Type> predicate, object value, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public TypePredicateCreationRule(Predicate<Type> predicate, Func<object> valueGenerator, int priority) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }
}
