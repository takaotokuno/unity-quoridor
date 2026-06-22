namespace Quoridor
{
    public sealed record MatchReadiedEvent: MatchEventBase
    {
        public MatchReadiedEvent()
            : base()
        {
            
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}