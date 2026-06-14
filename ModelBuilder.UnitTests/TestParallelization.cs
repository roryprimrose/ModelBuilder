// Several tests exercise the vNext registration mechanism by assigning the process-global
// typed-static slots (ValueSource<T>.Instance, ModelBuilderSlot<T>.Instance) and the shared
// Model.Registry / named-value-source registries. Those slots are static by design, so two test
// classes touching the same closed generic (for example ValueSource<int>) race when xUnit runs
// classes in parallel: a reader such as BuiltInValueSourcesTests can observe a constant source
// that NullableValueSourceTests installed, producing a rare, run-order-dependent failure.
//
// The suite completes in well under a second, so serialising it removes the entire static-slot
// race class — including for tests added later — at negligible cost.

using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
