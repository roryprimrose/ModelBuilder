// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    [Obsolete(DeprecationMessages.ValueSource, true)]
    public interface ITypeCreator
    {
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class TypeCreatorBase
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, Type type) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, ParameterInfo parameterInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, PropertyInfo propertyInfo) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, Type type, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public object Populate(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Create(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected Type ResolveBuildType(IBuildConfiguration buildConfiguration, Type requestedType) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AutoPopulate
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected IRandomGenerator Generator
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class ArrayTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateChildItem(Type type, IExecuteStrategy executeStrategy, object? previousItem) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AutoPopulate
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MaxCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MinCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class DefaultTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class EnumerableTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? Create(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateChildItem(Type type, IExecuteStrategy executeStrategy, object? previousItem) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public bool AutoPopulate
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MaxCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int MinCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class FactoryTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public FactoryTypeCreator(CacheLevel cacheLevel) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CacheLevel CacheLevel
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class SingletonTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public SingletonTypeCreator(CacheLevel cacheLevel) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public CacheLevel CacheLevel
        {
            get => throw new NotImplementedException();
        }

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }

    [Obsolete(DeprecationMessages.ValueSource, true)]
    [ExcludeFromCodeCoverage]
    public class StructTypeCreator
    {
        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        protected object PopulateInstance(IExecuteStrategy executeStrategy, object instance) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ValueSource, true)]
        [ExcludeFromCodeCoverage]
        public int Priority
        {
            get => throw new NotImplementedException();
        }
    }
}
