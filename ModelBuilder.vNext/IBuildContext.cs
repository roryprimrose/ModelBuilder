namespace ModelBuilder;

public interface IBuildContext
{
    Stack<IBuildStepResult> GetBuildChain();
    IReadOnlyCollection<IBuildStepResult> InstanceResults { get; }
    IBuildStep CurrentStep { get; }
    IBuildStep CurrentInstanceStep { get; }
    IBuildStepResult? CurrentInstance { get; }
}