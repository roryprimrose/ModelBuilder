namespace ModelBuilder;

public interface IBuildStep
{
    string? TargetName { get; }
    Type TargetType { get; }
    BuildStepType StepType { get; }
}