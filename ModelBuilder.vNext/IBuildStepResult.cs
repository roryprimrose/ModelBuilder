namespace ModelBuilder;

public interface IBuildStepResult : IBuildStep
{
    object? RawValue { get; }
}