namespace Quoridor
{
    public interface IEventSubscriber
    {
        void SubscribeTo(IMatchEventBus eventBus);
    }
}