namespace Quoridor
{
    public sealed class NewSessionResponse : GameResponseBase
    {
        public MatchSession Match { get; }
        public NewSessionResponse(MatchSession match) 
            : base(match != null, match != null ? "Match created" : "Failed to create match")
        {
            Match = match;   
        }
    }
}