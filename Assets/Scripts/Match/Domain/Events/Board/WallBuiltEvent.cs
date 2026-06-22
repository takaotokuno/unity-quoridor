using System.Collections.Generic;

namespace Quoridor
{
    public sealed record WallPlacedEvent : BoardTargetsEventBase
    {
        public WallPlacedEvent(IReadOnlyList<Position> targets)
            : base(targets)
        {
            
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}