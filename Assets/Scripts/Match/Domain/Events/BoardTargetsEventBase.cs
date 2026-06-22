using System.Collections.Generic;

namespace Quoridor
{
    public abstract record BoardTargetsEventBase : MatchEventBase
    {
        public IReadOnlyList<Position> Targets;
        protected BoardTargetsEventBase(IReadOnlyList<Position> targets)
        {
            Targets = targets;   
        }   
    }
}