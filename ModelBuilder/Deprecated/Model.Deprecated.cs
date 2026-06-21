// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <content>
    ///     v8 compatibility shims for members of <see cref="Model" /> that were removed or changed shape in
    ///     ModelBuilder vNext. These overloads exist only so a v8 consumer gets a compile error pointing at
    ///     the vNext replacement; they throw if reached.
    /// </content>
    public static partial class Model
    {
        [Obsolete(DeprecationMessages.ModelCreateArgs, true)]
        [ExcludeFromCodeCoverage]
        public static object Create(Type instanceType, params object?[]? args) => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelCreateArgs, true)]
        [ExcludeFromCodeCoverage]
        public static T Create<T>(params object?[]? args)
            where T : notnull
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelUsingDefaultConfiguration, true)]
        [ExcludeFromCodeCoverage]
        public static IBuildConfiguration UsingDefaultConfiguration() => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelUsingExecuteStrategy, true)]
        [ExcludeFromCodeCoverage]
        public static T UsingExecuteStrategy<T>()
            where T : IExecuteStrategy, new()
            => throw new NotImplementedException();

        [Obsolete(DeprecationMessages.ModelWriteLogGeneric, true)]
        [ExcludeFromCodeCoverage]
        public static IExecuteStrategy<T> WriteLog<T>(Action<string> action)
            where T : notnull
            => throw new NotImplementedException();
    }
}
