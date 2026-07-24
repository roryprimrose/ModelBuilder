namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="Model" /> class
    ///     is the static entry point for the v9 source-generated engine. It dispatches to the
    ///     generated builders through the typed-static slots on the generic path and through the
    ///     registry on the runtime-<see cref="Type" /> path.
    /// </summary>
    public static partial class Model
    {
        private static readonly ModelBuilderRegistry _registry = new ModelBuilderRegistry();
        private static readonly Dictionary<Type, Func<IBuildContext, object?>> _valueSourceFactories =
            new Dictionary<Type, Func<IBuildContext, object?>>();

        /// <summary>
        ///     Creates a populated instance of the specified type.
        /// </summary>
        /// <param name="instanceType">The type to create.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static object Create(Type instanceType)
        {
            instanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));

            var context = NewContext(null, null);

            using (context.EnterRoot(instanceType))
            {
                if (_registry.TryGet(instanceType, out var builder) && builder != null)
                {
                    return builder.Create(context);
                }

                if (_valueSourceFactories.TryGetValue(instanceType, out var factory))
                {
                    return factory(context)!;
                }

                if (BuiltInValueSources.Default.TryCreateBoxed(instanceType, context, out var builtInValue))
                {
                    return builtInValue!;
                }

                // Fall back to built-in type mappings (e.g. IReadOnlyDictionary<,> → Dictionary<,>)
                // so collection interfaces resolve without requiring the caller to declare them.
                if (context.Configuration.TryGetMapping(instanceType, out var mappedType))
                {
                    if (_registry.TryGet(mappedType, out var mappedBuilder) && mappedBuilder != null)
                    {
                        return mappedBuilder.Create(context);
                    }

                    if (_valueSourceFactories.TryGetValue(mappedType, out var mappedFactory))
                    {
                        return mappedFactory(context)!;
                    }
                }
            }

            throw NoBuilder(instanceType);
        }

        /// <summary>
        ///     Registers a value-source factory for a runtime type, so that <see cref="Create(Type)" />
        ///     can resolve value-source-only roots (collections, enums, nullable wrappers) that have no
        ///     generated class builder, without runtime reflection. Called by generated code; not
        ///     typically called directly.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> parameter is <c>null</c>.</exception>
        public static void RegisterValueSource<T>(IValueSource<T> source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            _valueSourceFactories[typeof(T)] = context => source.Create(context, new BuildTarget(typeof(T), typeof(T).Name));
        }

        /// <summary>
        ///     Creates a populated instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <returns>The created instance.</returns>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static T Create<T>()
        {
            return CreateWith<T>(null);
        }

        /// <summary>
        ///     Begins a configured build that ignores the specified member.
        /// </summary>
        /// <typeparam name="T">The type that declares the member to ignore.</typeparam>
        /// <param name="expression">An expression selecting the member to ignore.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public static IModelConfiguration Ignoring<T>(System.Linq.Expressions.Expression<Func<T, object?>> expression)
        {
            return new ModelConfiguration().Ignoring(expression);
        }

        /// <summary>
        ///     Begins a configured build that ignores the named member on the specified declaring type.
        /// </summary>
        /// <param name="declaringType">The type that declares the member to ignore.</param>
        /// <param name="memberName">The name of the member to ignore.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" /> or <paramref name="memberName" /> parameter is <c>null</c>.
        /// </exception>
        public static IModelConfiguration Ignoring(Type declaringType, string memberName)
        {
            return new ModelConfiguration().Ignoring(declaringType, memberName);
        }

        /// <summary>
        ///     Begins a configured build that ignores any member matching the predicate, across all types.
        /// </summary>
        /// <param name="predicate">The predicate evaluated against each member.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public static IModelConfiguration IgnoringAny(Func<MemberSignature, bool> predicate)
        {
            return new ModelConfiguration().IgnoringAny(predicate);
        }

        /// <summary>
        ///     Begins a configured build that maps a source type to a concrete target type.
        /// </summary>
        /// <typeparam name="TSource">The source type, typically an interface or abstract type.</typeparam>
        /// <typeparam name="TTarget">The concrete type to build in its place.</typeparam>
        /// <returns>A configuration to continue building.</returns>
        public static IModelConfiguration Mapping<TSource, TTarget>()
            where TTarget : TSource
        {
            return new ModelConfiguration().Mapping<TSource, TTarget>();
        }

        /// <summary>
        ///     Begins a configured build that maps a source type to a concrete target type.
        /// </summary>
        /// <param name="sourceType">The source type, typically an interface or abstract type.</param>
        /// <param name="targetType">The concrete type to build in its place.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="sourceType" /> or <paramref name="targetType" /> parameter is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     <paramref name="sourceType" /> and <paramref name="targetType" /> may both be open generic type
        ///     definitions (for example <c>typeof(IRepository&lt;&gt;)</c> mapped to <c>typeof(Repository&lt;&gt;)</c>),
        ///     letting one mapping serve every closed shape at build time. The generator still needs to see a
        ///     closed shape to generate a builder for it - declare each closed shape you need with its own
        ///     <see cref="Mapping{TSource, TTarget}" /> (or this overload with closed types) call.
        /// </remarks>
        public static IModelConfiguration Mapping(Type sourceType, Type targetType)
        {
            return new ModelConfiguration().Mapping(sourceType, targetType);
        }

        /// <summary>
        ///     Begins a typed construction of <typeparamref name="T" />. Supply constructor arguments
        ///     through the generated <c>From</c> extension method on the returned handle, for example
        ///     <c>Model.Construct&lt;Person&gt;().From("Fred", "Smith")</c>.
        /// </summary>
        /// <typeparam name="T">The type to construct.</typeparam>
        /// <returns>A handle whose generated <c>From</c> overloads name the constructors of <typeparamref name="T" />.</returns>
        /// <remarks>
        ///     The <c>From</c> overloads are generated for every public constructor of
        ///     <typeparamref name="T" /> at the call site, with typed parameters (no boxing, compile-time
        ///     validation) and the constructor's documentation copied across. Members assigned by the
        ///     chosen constructor are not re-populated with random values.
        /// </remarks>
        public static Construction<T> Construct<T>()
        {
            return new Construction<T>(null, null);
        }

        /// <summary>
        ///     Begins a configured build that registers a custom value source for every build target and root
        ///     of type <typeparamref name="T" />. A build target is a constructor parameter or settable member.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> parameter is <c>null</c>.</exception>
        public static IModelConfiguration AddValueSource<T>(IValueSource<T> source)
        {
            return new ModelConfiguration().AddValueSource(source);
        }

        /// <summary>
        ///     Begins a configured build that registers a custom value source for build targets of type
        ///     <typeparamref name="T" /> whose name matches one of <paramref name="names" />. A build target is
        ///     a constructor parameter or settable member.
        /// </summary>
        /// <typeparam name="T">The type the value source produces.</typeparam>
        /// <param name="source">The value source to register.</param>
        /// <param name="names">The names the source matches.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="source" /> or <paramref name="names" /> parameter is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="names" /> parameter is empty or contains a null, empty or whitespace entry.
        /// </exception>
        public static IModelConfiguration AddValueSource<T>(IValueSource<T> source, params string[] names)
        {
            return new ModelConfiguration().AddValueSource(source, names);
        }

        /// <summary>
        ///     Begins a configured build that tunes the build options, such as collection sizes, the
        ///     frequency of <c>null</c> values and the maximum graph depth.
        /// </summary>
        /// <param name="configure">An action that mutates the build options.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configure" /> parameter is <c>null</c>.</exception>
        public static IModelConfiguration SetOptions(Action<BuildOptions> configure)
        {
            return new ModelConfiguration().SetOptions(configure);
        }

        /// <summary>
        ///     Populates an existing instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to populate.</typeparam>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static T Populate<T>(T instance)
        {
            return PopulateWith(instance, null);
        }

        /// <summary>
        ///     Begins a configured build using the specified configuration module.
        /// </summary>
        /// <typeparam name="TModule">The configuration module to apply.</typeparam>
        /// <returns>A configuration to continue building.</returns>
        public static IModelConfiguration UsingModule<TModule>()
            where TModule : IConfigurationModule, new()
        {
            return new ModelConfiguration().UsingModule<TModule>();
        }

        /// <summary>
        ///     Begins a configured build that writes the structured build log to the supplied sink.
        /// </summary>
        /// <param name="sink">The action that receives the rendered build log after the build.</param>
        /// <returns>A configuration to continue building.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sink" /> parameter is <c>null</c>.</exception>
        public static IModelConfiguration WriteLog(Action<string> sink)
        {
            return new ModelConfiguration().WriteLog(sink);
        }

        internal static T CreateWith<T>(BuildConfiguration? configuration)
        {
            return CreateWith<T>(configuration, (IBuildLog?)null);
        }

        internal static T CreateWith<T>(BuildConfiguration? configuration, IBuildLog? log)
        {
            var context = NewContext(configuration, log);

            var builder = ModelBuilderSlot<T>.Instance;

            if (builder != null)
            {
                using (context.EnterRoot(typeof(T)))
                {
                    return builder.Create(context);
                }
            }

            // No generated model builder exists for T. Fall back to a registered value source so that
            // value-source-only roots such as Model.Create<int>() or Model.Create<SomeEnum>() work.
            using (context.EnterRoot(typeof(T)))
            {
                if (context.TryBuildRootValue<T>(out var value))
                {
                    return value;
                }

                // Fall back to built-in type mappings (e.g. IList<> → List<>) so collection
                // interfaces resolve without requiring the caller to declare them.
                if (context.Configuration.TryGetMapping(typeof(T), out var mappedType))
                {
                    if (_registry.TryGet(mappedType, out var mappedBuilder) && mappedBuilder != null)
                    {
                        return (T)mappedBuilder.Create(context);
                    }

                    if (_valueSourceFactories.TryGetValue(mappedType, out var mappedFactory))
                    {
                        return (T)mappedFactory(context)!;
                    }
                }
            }

            throw NoBuilder(typeof(T));
        }

        internal static T PopulateWith<T>(T instance, BuildConfiguration? configuration)
        {
            return PopulateWith(instance, configuration, null);
        }

        internal static T PopulateWith<T>(T instance, BuildConfiguration? configuration, IBuildLog? log)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var builder = ModelBuilderSlot<T>.Instance ?? throw NoBuilder(typeof(T));

            var context = NewContext(configuration, log);

            using (context.EnterRoot(typeof(T)))
            {
                return builder.Populate(context, instance);
            }
        }

        internal static T ConstructWith<T>(Func<IBuildContext, T> build, BuildConfiguration? configuration, IBuildLog? log)
        {
            var context = NewContext(configuration, log);

            using (context.EnterRoot(typeof(T)))
            {
                return build(context);
            }
        }

        private static BuildContext NewContext(BuildConfiguration? configuration, IBuildLog? log)
        {
            return new BuildContext(new RandomSource(), log, configuration?.Options, configuration);
        }

        private static ModelBuildException NoBuilder(Type type)
        {
            var message = "No builder was generated for '"
                          + type.FullName
                          + "'. Make it discoverable: call Model.Create<"
                          + type.Name
                          + ">() somewhere, add a Mapping<,> to it, or annotate it with [GenerateModelBuilder].";

            return new ModelBuildException(message, FailureKind.NoBuilderForType);
        }

        /// <summary>
        ///     Gets the registry that maps a runtime <see cref="Type" /> to its generated builder for the
        ///     non-generic <see cref="Create(Type)" /> path. The generated module initializer
        ///     registers each builder through this registry.
        /// </summary>
        public static IModelBuilderRegistry Registry => _registry;

        /// <summary>
        ///     Gets the concrete registry for engine-internal lookups (the read side is not part of the
        ///     public contract).
        /// </summary>
        internal static ModelBuilderRegistry RegistryInternal => _registry;

        /// <summary>
        ///     Attempts to create a value for the specified type using a registered value-source
        ///     factory. Used by <see cref="BuildContext" /> when a type mapping resolves to a
        ///     type that has a value source but no model builder (common for collections).
        /// </summary>
        /// <param name="type">The type to create a value for.</param>
        /// <param name="context">The build context.</param>
        /// <param name="value">The created value, when a factory exists.</param>
        /// <returns><c>true</c> if a factory was found and invoked; otherwise, <c>false</c>.</returns>
        internal static bool TryCreateFromValueSourceFactory(Type type, IBuildContext context, out object? value)
        {
            if (_valueSourceFactories.TryGetValue(type, out var factory))
            {
                value = factory(context);

                return true;
            }

            value = null;

            return false;
        }

        /// <summary>
        ///     Returns whether a value-source factory is registered for the specified type, without
        ///     invoking it. Used by <see cref="BuildContext" /> to test resolution before entering a
        ///     build frame.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if a factory is registered; otherwise, <c>false</c>.</returns>
        internal static bool HasValueSourceFactory(Type type)
        {
            return _valueSourceFactories.ContainsKey(type);
        }
    }
}
