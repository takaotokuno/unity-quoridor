namespace Quoridor
{
    public interface IMatchEventBus
    {
        void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent;
        void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent;
        void DispatchEvent<T>(T e) where T : IMatchEvent;
    }
}