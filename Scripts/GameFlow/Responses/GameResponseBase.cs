namespace Quoridor
{
    public abstract class GameResponseBase : IGameResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        
        protected GameResponseBase(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}