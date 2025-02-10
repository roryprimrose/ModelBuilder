namespace ModelBuilder
{
    public partial class Model
    {
        public static T Create<T>()
        {
            throw new InvalidOperationException(
                "A source generator should have been compiled into the calling project to generate the requested value");
        }
    }
}