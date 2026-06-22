using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    public sealed record PlaceWallCommand : MatchCommandBase
    {
        public IReadOnlyList<Position> Targets { get; }

        public PlaceWallCommand(IReadOnlyList<Position> targets, string issuer) 
            : base(issuer)
        {
            Targets = targets.ToArray();   
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}