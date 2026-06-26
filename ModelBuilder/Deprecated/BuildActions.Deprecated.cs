// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.BuildActions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using ModelBuilder.CreationRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class BuildCapability
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildCapability(IValueGenerator generator) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildCapability(ICreationRule rule) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public BuildCapability(ITypeCreator typeCreator, bool supportsCreate, bool supportsPopulate)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? CreateParameter(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? CreateProperty(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? CreateType(IExecuteStrategy executeStrategy, Type targetType, object?[]? args)
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public bool AutoPopulate
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public Type ImplementedByType
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public bool SupportsCreate
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public bool SupportsPopulate
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public enum BuildRequirement
    {
        Create,
        Populate
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class CircularReferenceBuildAction
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class CreationRuleBuildAction
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public interface IBuildAction
    {
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    public interface IBuildCapability
    {
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class TypeCreatorBuildAction
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.Engine, true)]
    [ExcludeFromCodeCoverage]
    public class ValueGeneratorBuildAction
    {
        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.Engine, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }
}
