namespace Quoridor
{
    public interface IMatchResponse
    {
        bool IsSuccess { get; }
        string Message { get; }        
    }
}