namespace Quoridor
{
    public abstract record BoardTargetEventBase : MatchEventBase
    {
        public readonly Position Target;
        protected BoardTargetEventBase(Position target)
        {
            Target = target;   
        }   
    }
}