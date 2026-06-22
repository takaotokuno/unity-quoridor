namespace Quoridor
{
    public sealed record RedoCommand : MatchCommandBase
    {
        public RedoCommand(string issuer)
            : base(issuer)
        {
            
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            throw new System.Exception();    
        }
    }   
}