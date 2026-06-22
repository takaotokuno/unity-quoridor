namespace Quoridor
{
    public abstract record MatchCommandBase : IMatchCommand
    {
        public string Issuer { get; }

        protected MatchCommandBase(string issuer)
        {
            Issuer = issuer;
        }

        public abstract CommandResult Execute(ICommandVisitor visitor);
    }
}