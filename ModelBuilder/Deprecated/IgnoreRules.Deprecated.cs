// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.IgnoreRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [Obsolete(DeprecationMessages.IgnoreRule, true)]
    public interface IIgnoreRule
    {
    }

    [Obsolete(DeprecationMessages.IgnoreRule, true)]
    [ExcludeFromCodeCoverage]
    public class ExpressionIgnoreRule<T>
    {
        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public ExpressionIgnoreRule(Expression<Func<T, object?>> expression) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.IgnoreRule, true)]
    [ExcludeFromCodeCoverage]
    public class PredicateIgnoreRule
    {
        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public PredicateIgnoreRule(Predicate<PropertyInfo> predicate) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.IgnoreRule, true)]
    [ExcludeFromCodeCoverage]
    public class RegexIgnoreRule
    {
        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexIgnoreRule(string expression) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public RegexIgnoreRule(Regex expression) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public bool IsMatch(PropertyInfo propertyInfo) => throw new NotImplementedException();
    }
}
