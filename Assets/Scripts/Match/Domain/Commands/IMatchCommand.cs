namespace Quoridor
{
    public interface IMatchCommand
    {
        CommandResult Execute(ICommandVisitor visitor);        
    }   
}