namespace Quoridor
{
    public sealed class GameErrorResponse : GameResponseBase
    {
        public GameErrorResponse(string message) 
            : base(false, message)
        {
            
        }
    }
}