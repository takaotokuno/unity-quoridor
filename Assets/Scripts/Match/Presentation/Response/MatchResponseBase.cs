namespace Quoridor
{
    public abstract class MatchResponseBase : IMatchResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        
        protected MatchResponseBase(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}