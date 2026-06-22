namespace Quoridor
{
    public sealed class ResetResponse : GameResponseBase
    {
        public ResetResponse() 
            : base(true, "Reset completed")
        {
            
        }
    }
}