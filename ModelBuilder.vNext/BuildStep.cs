namespace ModelBuilder;

public record BuildStep(BuildStepType StepType, Type TargetType, string? TargetName) : IBuildStep;