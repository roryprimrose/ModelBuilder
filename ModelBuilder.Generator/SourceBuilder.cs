namespace ModelBuilder.Generator
{
    using System.Collections.Immutable;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal class SourceBuilder
    {
        private readonly ITypeStore _typeStore;

        private readonly string _version;

        public SourceBuilder(ITypeStore typeStore)
        {
            _typeStore = typeStore;

            var assembly = GetType().Assembly;
            _version = assembly.GetName().Version.ToString() ?? "1.0";
        }

        public void Build(SourceProductionContext context, ImmutableArray<BindTarget?> bindTargets)
        {
            // Separate the bind targets into groups based on the calling namespace
            var callersByNamespace = bindTargets.GroupBy(x => x?.CallingNamespace);

            foreach (var caller in callersByNamespace)
            {
                if (caller.Key == null)
                {
                    continue;
                }

                // Get the distinct target types for the calling namespace
                var targetTypes = caller.Select(x => new TargetType(x.TypeNamespace, x.TypeName)).Distinct().ToList();

                WriteCreateProxy(context, caller.Key, targetTypes);
            }

            // Get the unique bind targets regardless of which calling namespace calls for a Create<T>()
            var uniqueTargetTypes = bindTargets.Select(x => new TargetType(x.TypeNamespace, x.TypeName)).Distinct()
                .ToList();

            WriteTypeBuilders(context, uniqueTargetTypes!);
        }

        private void WriteCreateProxy(SourceProductionContext context, string callingNamespace,
            ICollection<TargetType?> targetTypes)
        {
            var callerNamespace = callingNamespace;
            var hasNamespace = string.IsNullOrWhiteSpace(callerNamespace) == false;

            // TODO: Make this namespace relative to the calling project
            var builderNamespace = typeof(ModelCreateIncrementalGenerator).Namespace! + ".SourceGenerator_Builders";

            var builder = new StringBuilder();
            var padding = string.Empty;

            if (hasNamespace)
            {
                padding = "    ";
                builder.AppendLine($"namespace {callerNamespace}");
                builder.AppendLine("{");
            }

            builder.AppendLine(
                $"{padding}[System.CodeDom.Compiler.GeneratedCode(\"{typeof(ModelCreateIncrementalGenerator)}\", \"{_version}\")]");
            builder.AppendLine($"{padding}[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
            builder.AppendLine($"{padding}public static partial class Model");
            builder.AppendLine($"{padding}{{");

            // Write the proxy method that will handle the transition from T to target type
            builder.AppendLine($"{padding}    public static T Create<T>()");
            builder.AppendLine($"{padding}    {{");

            foreach (var targetType in targetTypes)
            {
                if (targetType == null)
                {
                    continue;
                }

                // Get the definition from the cache
                var definition = _typeStore.GetDefinition(targetType);

                builder.AppendLine($"{padding}        if (typeof(T) == typeof({definition.FullTypeName}))");
                builder.AppendLine($"{padding}        {{");
                builder.AppendLine(
                    $"{padding}            // Custom initialization logic for {definition.FullTypeName}");
                builder.AppendLine(
                    $"{padding}            return (T)(object)({builderNamespace}.Builder.Create_{definition.SafeTypeName}());");
                builder.AppendLine($"{padding}        }}");
                builder.AppendLine();
            }

            builder.AppendLine(
                $"{padding}        // The type requested was not included in this source generated code");
            builder.AppendLine(
                $"{padding}        throw new System.InvalidOperationException($\"The requested type {{typeof(T)}} does not have source generator code created for it\");");
            builder.AppendLine($"{padding}    }}");

            if (hasNamespace)
            {
                builder.AppendLine($"{padding}}}");
            }

            builder.AppendLine("}");

            var source = builder.ToString();

            var filename = _typeStore.CreateSafeName(callerNamespace, "Global");

            filename += "_CreateProxy.g.cs";

            context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
        }

        private void WriteTypeBuilders(SourceProductionContext context, List<TargetType?> targetTypes)
        {
            var builder = new StringBuilder();

            // TODO: Make this namespace relative to the calling project
            var builderNamespace = typeof(ModelCreateIncrementalGenerator).Namespace! + ".SourceGenerator_Builders";

            var padding = "    ";
            builder.AppendLine($"namespace {builderNamespace}");
            builder.AppendLine("{");

            builder.AppendLine(
                $"{padding}[System.CodeDom.Compiler.GeneratedCode(\"{typeof(ModelCreateIncrementalGenerator)}\", \"{_version}\")]");
            builder.AppendLine($"{padding}public static class Builder");
            builder.AppendLine($"{padding}{{");

            foreach (var targetType in targetTypes)
            {
                if (targetType == null)
                {
                    continue;
                }

                // Get the definition from the cache
                var definition = _typeStore.GetDefinition(targetType);

                // Write the proxy method that will handle the transition from T to target type
                builder.AppendLine(
                    $"{padding}    public static {definition.FullTypeName} Create_{definition.SafeTypeName}()");
                builder.AppendLine($"{padding}    {{");

                foreach (var targetParameter in definition.Parameters)
                {
                    builder.AppendLine(
                        $"{padding}        // Custom initialization logic for constructor parameter {targetParameter.ParameterName}");
                }

                builder.AppendLine($"{padding}        var constructedValue = new {definition.FullTypeName}();");

                foreach (var targetProperty in definition.Properties)
                {
                    builder.AppendLine(
                        $"{padding}        // Custom initialization logic for property {targetProperty.PropertyName}");
                }

                builder.AppendLine($"{padding}        return constructedValue;");

                builder.AppendLine($"{padding}    }}");
                builder.AppendLine();
            }

            builder.AppendLine($"{padding}}}");

            builder.AppendLine("}");

            var source = builder.ToString();

            var filename = _typeStore.CreateSafeName(builderNamespace + ".Builder.g.cs");

            context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
        }
    }
}