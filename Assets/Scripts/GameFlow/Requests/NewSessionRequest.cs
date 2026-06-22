namespace Quoridor
{
    public sealed class NewSessionRequest : GameRequestBase
    {
        public MatchSetting Setting { get; }
        public NewSessionRequest(MatchSetting setting) 
            : base()
        {
            Setting = setting;
        }   
    }   
}