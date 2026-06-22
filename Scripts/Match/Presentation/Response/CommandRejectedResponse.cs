namespace Quoridor
{
    public sealed class CommandRejectedResponse : MatchResponseBase
    {
        public CommandRejectedResponse(string message) 
            : base(false, message)
        {
            
        }
    }
}