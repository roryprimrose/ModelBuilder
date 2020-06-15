namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.BuildActions;

    /// <summary>
    ///     The <see cref="BuildHistoryItem" />
    ///     class is used to track items built and related build capabilities.
    /// </summary>
    public class BuildHistoryItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildHistoryItem" /> class.
        /// </summary>
        /// <param name="value">The value created.</param>
        public BuildHistoryItem(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     The cache of build capabilities by requested types.
        /// </summary>
        public Dictionary<Type, IBuildCapability> Capabilities { get; } = new Dictionary<Type, IBuildCapability>();

        /// <summary>
        ///     Gets the value created.
        /// </summary>
        public object Value { get; }
    }
}