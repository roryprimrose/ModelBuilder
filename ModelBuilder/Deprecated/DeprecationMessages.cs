// Deprecated v8 compatibility shims. See .editorconfig in this folder.
#pragma warning disable CS0612, CS0618, CS0619
#nullable enable

namespace ModelBuilder
{
    /// <summary>
    ///     Centralised migration messages used by the <c>[Obsolete]</c> attributes on the v8 compatibility
    ///     shims. Each constant points a v8 consumer at the ModelBuilder vNext replacement.
    /// </summary>
    internal static class DeprecationMessages
    {
        internal const string Engine =
            "The runtime build pipeline has been replaced by source-generated builders in ModelBuilder vNext. "
            + "This engine type has been removed. Use Model.Create<T>() or Model.Construct<T>().From(...).";

        internal const string Resolver =
            "Construction and member resolution are performed by the source generator in ModelBuilder vNext. "
            + "This resolver has been removed.";

        internal const string ExecuteStrategy =
            "Execute strategies have been removed in ModelBuilder vNext. Use Model.Create<T>() and the "
            + "source-generated builders.";

        internal const string ValueSource =
            "Value generators and type creators have been replaced by IValueSource<T> in ModelBuilder vNext. "
            + "Register one with Model.AddValueSource<T>(...) or IModelConfiguration.AddValueSource<T>(...).";

        internal const string IgnoreRule =
            "Use Model.Ignoring<T>(...) or IModelConfiguration.Ignoring<T>(...) in ModelBuilder vNext.";

        internal const string ExecuteOrder =
            "Member build order is determined by the source generator in ModelBuilder vNext; execute-order "
            + "rules have been removed.";

        internal const string CreationRule =
            "Register a custom IValueSource<T> with Model.AddValueSource<T>(source, memberNames) in "
            + "ModelBuilder vNext.";

        internal const string Mapping =
            "Use Model.Mapping<TSource, TTarget>() or IModelConfiguration.Mapping<TSource, TTarget>() in "
            + "ModelBuilder vNext.";

        internal const string Random =
            "Use IRandomSource (RandomSource) in ModelBuilder vNext instead of IRandomGenerator.";

        internal const string Configuration =
            "Use IModelConfiguration via the Model entry points (Ignoring, Mapping, UsingModule, "
            + "AddValueSource, WriteLog) in ModelBuilder vNext.";

        internal const string BuildLog =
            "Use Model.WriteLog(Action<string>) in ModelBuilder vNext to capture the build log.";

        internal const string Module =
            "Implement IConfigurationModule (vNext shape) and apply it with Model.UsingModule<T>(); defaults "
            + "are implicit in ModelBuilder vNext.";

        internal const string Cache =
            "Caching is handled internally by the source generator in ModelBuilder vNext; CacheLevel has been "
            + "removed.";

        internal const string NameExpression =
            "Name-based member matching is configured through AddValueSource<T>(source, memberNames) in "
            + "ModelBuilder vNext; NameExpression has been removed.";

        internal const string Nullable =
            "Nullability is handled by the source generator in ModelBuilder vNext; INullableBuilder has been "
            + "removed.";

        internal const string Common =
            "These helpers have been removed in ModelBuilder vNext. Set<T> remains on SetExtensions and "
            + "per-item iteration remains on SetEachExtensions.";

        internal const string Expression =
            "Expression helpers have been removed in ModelBuilder vNext; use System.Linq.Expressions directly.";

        internal const string BuildConfigurationExtensions =
            "Configuration mutation extensions have been removed in ModelBuilder vNext. Use the Model entry "
            + "points and IModelConfiguration (AddValueSource, Ignoring, Mapping, UsingModule, WriteLog).";

        internal const string PostBuildAction =
            "Post-build actions have been removed in ModelBuilder vNext.";

        internal const string BuildException =
            "Use ModelBuildException in ModelBuilder vNext.";

        internal const string ModelCreateArgs =
            "Use Model.Construct<T>().From(...) to supply constructor arguments in ModelBuilder vNext.";

        internal const string ModelUsingDefaultConfiguration =
            "Defaults are implicit in ModelBuilder vNext; start from the Model entry points directly.";

        internal const string ModelUsingExecuteStrategy =
            "Execute strategies have been removed in ModelBuilder vNext; use Model.Create<T>().";

        internal const string ModelWriteLogGeneric =
            "Use Model.WriteLog(Action<string>), which returns IModelConfiguration, in ModelBuilder vNext.";
    }
}
