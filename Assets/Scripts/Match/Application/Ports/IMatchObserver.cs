namespace Quoridor
{
    public interface IMatchObserver<T> where T : IMatchEvent
    {
        void Notify(T e);
    }
}