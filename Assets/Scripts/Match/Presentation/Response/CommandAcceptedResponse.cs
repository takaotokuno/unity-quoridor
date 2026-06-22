namespace Quoridor
{
    public sealed class CommandAcceptedResponse : MatchResponseBase
    {
        public CommandAcceptedResponse() 
            : base(true, "Success")
        {
            
        }
    }
}