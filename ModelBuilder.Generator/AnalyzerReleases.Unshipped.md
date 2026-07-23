; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
MB1001  | ModelBuilder | Warning | Abstract or interface build root has no mapping
MB1002  | ModelBuilder | Warning | Build root has no accessible constructor
MB1005  | ModelBuilder | Warning | Model.Create(typeof(X)) names a type that cannot be built
MB1006  | ModelBuilder | Warning | Open generic mapping target has no accessible constructor
MB1007  | ModelBuilder | Warning | Open generic mapping is never used in closed form
MB1011  | ModelBuilder | Warning | Discovered collection shape is not supported
