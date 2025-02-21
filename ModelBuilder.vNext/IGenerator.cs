namespace ModelBuilder;

public interface IGenerator
{
    bool CanCreate();

    bool CanPopulate();

    T Create<T>();

    T Populate<T>(T value);

    int Priority { get; }
}