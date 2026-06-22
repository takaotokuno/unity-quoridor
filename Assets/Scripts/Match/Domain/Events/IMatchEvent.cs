namespace Quoridor
{
    public interface IMatchEvent
    {
       void Dispatch(IMatchEventBus bus);
    }
}