namespace Quoridor
{
    public sealed record SkipCommand : PlayerCommandBase
    {
        public SkipCommand(PlayerId playerId, string issuer) 
            : base(playerId, issuer)
        {
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}