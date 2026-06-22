namespace Quoridor
{
    public interface IMatchCommandPort
    {
        IMatchResponse DispatchCommand(IMatchCommand command);  
    }
}