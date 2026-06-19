namespace ModelBuilder.Generator
{
    /// <summary>
    ///     The <see cref="ConstructionRequest" /> struct
    ///     records a <c>Model.Construct&lt;T&gt;()</c> call site: the fully qualified type to construct
    ///     and the namespace the call appears in, so the generator can emit the typed <c>From</c>
    ///     extension overloads into that namespace.
    /// </summary>
    internal readonly struct ConstructionRequest
    {
        public ConstructionRequest(string typeFullName, string callingNamespace)
        {
            TypeFullName = typeFullName;
            CallingNamespace = callingNamespace;
        }

        public string CallingNamespace { get; }

        public string TypeFullName { get; }
    }
}
