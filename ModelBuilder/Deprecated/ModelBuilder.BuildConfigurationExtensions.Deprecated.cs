// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    [Obsolete(DeprecationMessages.BuildConfigurationExtensions, true)]
    [ExcludeFromCodeCoverage]
    public static class BuildConfigurationExtensions
    {
        [Obsolete(DeprecationMessages.Module, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IConfigurationModule module)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, ICreationRule rule)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IExecuteOrderRule rule)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IIgnoreRule rule)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.PostBuildAction, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IPostBuildAction postBuildAction)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, ITypeCreator typeCreator)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, TypeMappingRule rule)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IValueGenerator valueGenerator)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule<T>(this IBuildConfiguration configuration, Expression<Func<T, object?>> expression, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<Type> predicate, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<Type> predicate, Func<object> valueGenerator, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<PropertyInfo> predicate, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<PropertyInfo> predicate, Func<object> valueGenerator, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<ParameterInfo> predicate, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Predicate<ParameterInfo> predicate, Func<object> valueGenerator, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Type targetType, Regex expression, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddCreationRule(this IBuildConfiguration configuration, Type targetType, string expression, object value, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddExecuteOrderRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddExecuteOrderRule<T>(this IBuildConfiguration configuration, Expression<Func<T, object?>> expression, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddExecuteOrderRule(this IBuildConfiguration configuration, Regex expression, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddExecuteOrderRule(this IBuildConfiguration configuration, Predicate<PropertyInfo> predicate, int priority)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddIgnoreRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddIgnoreRule<T>(this IBuildConfiguration configuration, Expression<Func<T, object?>> expression)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddIgnoreRule(this IBuildConfiguration configuration, Predicate<PropertyInfo> predicate)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddIgnoreRule(this IBuildConfiguration configuration, Regex expression)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddIgnoreRule(this IBuildConfiguration configuration, string expression)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.PostBuildAction, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddPostBuildAction<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddTypeCreator<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddTypeMappingRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddTypeMappingRule<TSource, TTarget>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration AddValueGenerator<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelCreateArgs, true)]
        [ExcludeFromCodeCoverage]
        public static T Create<T>(this IBuildConfiguration buildConfiguration, params object?[]? args)
            where T : notnull
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelCreateArgs, true)]
        [ExcludeFromCodeCoverage]
        public static object Create(this IBuildConfiguration buildConfiguration, Type instanceType, params object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Ignoring<T>(this IBuildConfiguration buildConfiguration, Expression<Func<T, object?>> expression)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration Mapping<TSource, TTarget>(this IBuildConfiguration buildConfiguration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public static T Populate<T>(this IBuildConfiguration buildConfiguration, T instance)
            where T : notnull
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveCreationRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveExecuteOrderRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveIgnoreRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.PostBuildAction, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemovePostBuildAction<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveTypeCreator<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Mapping, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveTypeMappingRule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration RemoveValueGenerator<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.CreationRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdateCreationRule<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ExecuteOrder, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdateExecuteOrderRule<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.IgnoreRule, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdateIgnoreRule<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.PostBuildAction, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdatePostBuildAction<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdateTypeCreator<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UpdateValueGenerator<T>(this IBuildConfiguration configuration, Action<T> action)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelUsingExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public static T UsingExecuteStrategy<T>(this IBuildConfiguration buildConfiguration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Module, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UsingModule<T>(this IBuildConfiguration configuration)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelWriteLogGeneric, true)]
        [ExcludeFromCodeCoverage]
        public static IExecuteStrategy<T> WriteLog<T>(this IBuildConfiguration configuration, Action<string> action)
            where T : notnull
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.BuildLog, true)]
        [ExcludeFromCodeCoverage]
        public static IExecuteStrategy WriteLog(this IBuildConfiguration configuration, Action<string> action)
            => throw new NotImplementedException();
    }
}
