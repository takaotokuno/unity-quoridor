namespace Quoridor
{
    public sealed record MatchStartCommand : MatchCommandBase
    {
        public MatchStartCommand(string issuer) 
            : base(issuer)
        {

        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}