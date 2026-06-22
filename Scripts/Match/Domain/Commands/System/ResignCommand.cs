namespace Quoridor
{
    public sealed record ResignCommand : PlayerCommandBase
    {
        public ResignCommand(PlayerId playerId, string issuer) 
            : base(playerId, issuer)
        {
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}