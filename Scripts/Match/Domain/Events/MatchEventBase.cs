namespace Quoridor
{
    public abstract record MatchEventBase : IMatchEvent
    {
        protected MatchEventBase(){}
        public abstract void Dispatch(IMatchEventBus bus);
    }
}