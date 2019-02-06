namespace ModelBuilder
{
    public interface IBuildHistory : IBuildChain
    {
        void Pop();

        void Push(object instance);
    }
}