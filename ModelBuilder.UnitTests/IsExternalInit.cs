// the following namespace/class is required to be able to use init setters with framework versions lower than 5.0:
// https://www.mking.net/blog/error-cs0518-isexternalinit-not-defined

namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}