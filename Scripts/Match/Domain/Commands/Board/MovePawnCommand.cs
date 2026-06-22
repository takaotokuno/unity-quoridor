namespace Quoridor
{
    public sealed record MovePawnCommand : PlayerCommandBase
    {
        public Position To { get; }

        public MovePawnCommand(PlayerId playerId, Position to, string issuer) 
            : base(playerId, issuer)
        {
            To = to;   
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}