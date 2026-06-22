namespace Quoridor
{
    public interface IGameResponse
    {
        bool IsSuccess { get; }
        string Message { get; }        
    }
}