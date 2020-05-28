namespace ModelBuilder.UnitTests.Models
{
    public class ParameterModel<T>
    {
        public ParameterModel(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}