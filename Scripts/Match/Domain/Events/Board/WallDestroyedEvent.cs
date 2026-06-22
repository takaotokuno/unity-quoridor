using System.Collections.Generic;

namespace Quoridor
{
    public sealed record WallRemovedEvent : BoardTargetsEventBase
    {
        public WallRemovedEvent(IReadOnlyList<Position> targets)
            : base(targets)
        {
            
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}