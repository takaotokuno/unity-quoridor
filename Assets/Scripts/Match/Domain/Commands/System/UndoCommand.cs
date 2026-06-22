namespace Quoridor
{
    public sealed record UndoCommand : MatchCommandBase
    {
        public UndoCommand(string issuer)
            : base(issuer)
        {
            
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            throw new System.Exception();    
        }
    }   
}