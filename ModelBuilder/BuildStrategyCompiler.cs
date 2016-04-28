using System;
using System.Collections.Generic;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BuildStrategyCompiler"/>
    /// class is used to assist in creating a new <see cref="IBuildStrategy"/> instance.
    /// </summary>
    public class BuildStrategyCompiler : IBuildStrategyCompiler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStrategyCompiler"/> class.
        /// </summary>
        public BuildStrategyCompiler()
        {
            ExecuteOrderRules = new List<ExecuteOrderRule>();
            IgnoreRules = new List<IgnoreRule>();
            TypeCreators = new List<ITypeCreator>();
            ValueGenerators = new List<IValueGenerator>();
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">The <see cref="ConstructorResolver"/> property is <c>null</c>.</exception>
        public IBuildStrategy Compile()
        {
            if (ConstructorResolver == null)
            {
                throw new InvalidOperationException(Resources.BuildStrategyCompiler_NullConstructorResolver);
            }

            return new BuildStrategy(ConstructorResolver, TypeCreators, ValueGenerators, IgnoreRules, ExecuteOrderRules);
        }

        /// <inheritdoc />
        public IConstructorResolver ConstructorResolver { get; set; }

        /// <inheritdoc />
        public ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <inheritdoc />
        public ICollection<IgnoreRule> IgnoreRules { get; }

        /// <inheritdoc />
        public ICollection<ITypeCreator> TypeCreators { get; }

        /// <inheritdoc />
        public ICollection<IValueGenerator> ValueGenerators { get; }
    }
}