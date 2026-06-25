// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.BuildActions;

    [Obsolete(DeprecationMessages.BuildException, true)]
    [ExcludeFromCodeCoverage]
    public class BuildException : Exception
    {
        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public BuildException() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public BuildException(string message) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public BuildException(string message, Exception inner) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public BuildException(string message, Type? targetType, string? referenceName, object? context, string? buildLog)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public BuildException(
            string message,
            Type? targetType,
            string? referenceName,
            object? context,
            string? buildLog,
            Exception? inner)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public string? BuildLog
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public object? Context
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public string? ReferenceName
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.BuildException, true)]
        [ExcludeFromCodeCoverage]
        public Type? TargetType
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class BuildHistory
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public void AddCapability(Type type, IBuildCapability capability) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetCapability(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IEnumerator GetEnumerator() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public void Pop() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public void Push(object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public int Count
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? First
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Last
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class BuildHistoryItem
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildHistoryItem(object value) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public Dictionary<Type, IBuildCapability> Capabilities
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Value
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class BuildProcessor
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildProcessor() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildProcessor(IEnumerable<IBuildAction> actions) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy, BuildRequirement buildRequirement, Type type)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy, BuildRequirement buildRequirement, ParameterInfo parameterInfo)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy, BuildRequirement buildRequirement, PropertyInfo propertyInfo)
            => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Cache, true)]
    public enum CacheLevel
    {
        None = 0,
        PerInstance,
        Global
    }

    [Obsolete(DeprecationMessages.Common, true)]
    [ExcludeFromCodeCoverage]
    public static class CommonExtensions
    {
        [Obsolete(DeprecationMessages.Common, true)]
        [ExcludeFromCodeCoverage]
        public static bool IsNullable(this Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Common, true)]
        [ExcludeFromCodeCoverage]
        public static T Next<T>(this IReadOnlyList<T> source) => throw new NotImplementedException();

        // Note: the v8 Set<T>(this T, Action<T>) overload is intentionally NOT redeclared here. It
        // remains a live v9 API on SetExtensions, so a deprecated stub with the identical signature
        // would make every call to .Set(...) ambiguous (CS0121) for consumers.
        [Obsolete(DeprecationMessages.Common, true)]
        [ExcludeFromCodeCoverage]
        public static T Set<T, TVALUE>(this T instance, Expression<Func<T, TVALUE>> expressionFunc, TVALUE value)
            => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.BuildLog, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultBuildLog
    {
        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void BuildFailure(Exception ex) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CircularReferenceDetected(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void Clear() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatedParameter(ParameterInfo parameterInfo, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatedProperty(PropertyInfo propertyInfo, object context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatedType(Type type, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatingParameter(ParameterInfo parameterInfo, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatingProperty(PropertyInfo propertyInfo, object context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatingType(Type type, Type creatorType, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void CreatingValue(Type type, Type generatorType, object? context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void IgnoringProperty(PropertyInfo propertyInfo, object context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void MappedType(Type source, Type target) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void PopulatedInstance(object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void PopulatingInstance(object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public void PostBuildAction(Type type, Type postBuildType, object context) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        protected virtual void WriteMessage(string message) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public bool IsEnabled
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public string Output
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Module, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultConfigurationModule
    {
        [Obsolete(DeprecationMessages.Module, true)]
        [ExcludeFromCodeCoverage]
        public void Configure(IBuildConfiguration configuration) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultConstructorResolver
    {
        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public DefaultConstructorResolver(CacheLevel cacheLevel) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public ConstructorInfo? Resolve(Type type, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public CacheLevel CacheLevel
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultExecuteStrategy
    {
        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public DefaultExecuteStrategy() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public DefaultExecuteStrategy(IBuildHistory buildHistory, IBuildLog buildLog, IBuildProcessor buildProcessor)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public object Create(Type type, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public object?[]? CreateParameters(MethodBase method) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public void Initialize(IBuildConfiguration configuration) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public virtual object Populate(object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        protected virtual object Build(Type type, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        protected virtual object? Build(ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        protected virtual object? Build(PropertyInfo propertyInfo, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        protected virtual void PopulateProperty(PropertyInfo propertyInfo, object instance, params object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public IBuildChain BuildChain
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public IBuildConfiguration Configuration
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public IBuildLog Log
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultExecuteStrategy<T>
        where T : notnull
    {
        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public DefaultExecuteStrategy() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public DefaultExecuteStrategy(IBuildHistory buildHistory, IBuildLog buildLog, IBuildProcessor buildProcessor)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public virtual T Create(params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public virtual T Populate(T instance) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultParameterResolver
    {
        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public DefaultParameterResolver(CacheLevel cacheLevel) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public IEnumerable<ParameterInfo> GetOrderedParameters(IBuildConfiguration configuration, MethodBase method)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public CacheLevel CacheLevel
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultPropertyResolver
    {
        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public DefaultPropertyResolver(CacheLevel cacheLevel) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public IEnumerable<PropertyInfo> GetOrderedProperties(IBuildConfiguration configuration, Type targetType)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public virtual bool IsIgnored(IBuildConfiguration configuration, object instance, PropertyInfo propertyInfo, object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public CacheLevel CacheLevel
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultTypeResolver
    {
        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        public Type GetBuildType(IBuildConfiguration configuration, Type requestedType) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Resolver, true)]
        [ExcludeFromCodeCoverage]
        protected virtual IEnumerable<Type> GetTypesToEvaluate(Type requestedType) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
    [ExcludeFromCodeCoverage]
    public static class ExecuteStrategyExtensions
    {
        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public static IExecuteStrategy WriteLog(this IExecuteStrategy executeStrategy, Action<string> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public static IExecuteStrategy<T> WriteLog<T>(this IExecuteStrategy<T> executeStrategy, Action<string> action)
            where T : notnull
            => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Expression, true)]
    [ExcludeFromCodeCoverage]
    public static class ExpressionExtensions
    {
        [Obsolete(DeprecationMessages.Expression, true)]
        [ExcludeFromCodeCoverage]
        public static PropertyInfo GetProperty<T>(this Expression<Func<T, object?>> expression)
            => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public interface IBuildChain : IEnumerable<object>
    {
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public interface IBuildHistory
    {
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public interface IBuildProcessor
    {
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    public interface IConstructorResolver
    {
    }

    [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
    public interface IExecuteStrategy
    {
    }

    [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
    public interface IExecuteStrategy<T>
        where T : notnull
    {
    }

    [Obsolete(DeprecationMessages.Nullable, true)]
    public interface INullableBuilder
    {
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    public interface IParameterResolver
    {
    }

    [Obsolete(DeprecationMessages.PostBuildAction, true)]
    public interface IPostBuildAction
    {
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    public interface IPropertyResolver
    {
    }

    [Obsolete(DeprecationMessages.Random, true)]
    public interface IRandomGenerator
    {
    }

    [Obsolete(DeprecationMessages.Resolver, true)]
    public interface ITypeResolver
    {
    }

    [Obsolete(DeprecationMessages.NameExpression, true)]
    [ExcludeFromCodeCoverage]
    public static class NameExpression
    {
        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex Age
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex City
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex Country
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex DateOfBirth
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex Domain
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex Email
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex FirstName
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex Gender
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex LastName
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex MiddleName
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex PostCode
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex State
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.NameExpression, true)]
        [ExcludeFromCodeCoverage]
        public static Regex TimeZone
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Random, true)]
    [ExcludeFromCodeCoverage]
    public class RandomGenerator
    {
        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public object GetMax(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public object GetMin(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public virtual bool IsSupported(Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public object NextValue(Type type, object min, object max) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public void NextValue(byte[] buffer) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Random, true)]
    [ExcludeFromCodeCoverage]
    public static class RandomGeneratorExtensions
    {
        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public static T NextValue<T>(this IRandomGenerator generator)
            where T : struct
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public static T NextValue<T>(this IRandomGenerator generator, T max)
            where T : struct
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public static T NextValue<T>(this IRandomGenerator generator, T min, T max)
            where T : struct
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public static object NextValue(this IRandomGenerator generator, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Random, true)]
        [ExcludeFromCodeCoverage]
        public static object NextValue(this IRandomGenerator generator, Type type, object max) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.Mapping, true)]
    [ExcludeFromCodeCoverage]
    public class TypeMappingRule
    {
        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public TypeMappingRule(Type sourceType, Type targetType) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public Type SourceType
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public Type TargetType
        {
            get => throw new NotImplementedException();
        }
    }
}
