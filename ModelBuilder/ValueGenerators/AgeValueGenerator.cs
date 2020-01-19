﻿namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="AgeValueGenerator" />
    ///     class is used to generate numbers that should represent a persons age.
    /// </summary>
    public class AgeValueGenerator : NumericValueGenerator
    {
        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
        {
            var baseSupported = base.IsSupported(type, referenceName, buildChain);

            if (baseSupported == false)
            {
                return false;
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                return false;
            }

            if (referenceName.IndexOf("age", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override object GetMaximum(Type type, string referenceName, object context)
        {
            return MaxAge;
        }

        /// <inheritdoc />
        protected override object GetMinimum(Type type, string referenceName, object context)
        {
            return 1;
        }

        /// <summary>
        ///     Gets or sets the default maximum age that can be generated.
        /// </summary>
        public static int DefaultMaxAge { get; set; } = 100;

        /// <summary>
        ///     Gets or sets the maximum age generated by this instance.
        /// </summary>
        public int MaxAge { get; set; } = DefaultMaxAge;

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}