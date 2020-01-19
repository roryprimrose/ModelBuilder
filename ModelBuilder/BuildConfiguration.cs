﻿namespace ModelBuilder
{
    using System.Collections.Generic;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildConfiguration" />
    ///     class is used to contain all the configuration required to create values.
    /// </summary>
    public class BuildConfiguration : IBuildConfiguration
    {
        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; set; } = new DefaultConstructorResolver();

        /// <inheritdoc />
        public ICollection<CreationRule> CreationRules { get; } = new List<CreationRule>();

        /// <inheritdoc />
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; } = new List<ExecuteOrderRule>();

        /// <inheritdoc />
        public ICollection<IgnoreRule> IgnoreRules { get; } = new List<IgnoreRule>();

        /// <inheritdoc />
        public ICollection<IPostBuildAction> PostBuildActions { get; } = new List<IPostBuildAction>();

        /// <inheritdoc />
        public IPropertyResolver PropertyResolver { get; set; } = new DefaultPropertyResolver();

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; } = new List<ITypeCreator>();

        /// <inheritdoc />
        public ICollection<TypeMappingRule> TypeMappingRules { get; } = new List<TypeMappingRule>();

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; } = new List<IValueGenerator>();
    }
}