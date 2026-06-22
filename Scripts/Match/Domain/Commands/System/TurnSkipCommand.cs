namespace Quoridor
{
    public sealed record TurnSkipCommand : PlayerCommandBase
    {
        public TurnSkipCommand(PlayerId playerId, string issuer)
            : base(playerId, issuer)
        {
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
