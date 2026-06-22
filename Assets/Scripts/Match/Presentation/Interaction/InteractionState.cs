namespace Quoridor
{
    public sealed class InteractionState
    {
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public bool IsPreviewed { get; set; }
        public int RemainingUses { get; set; }

        public void Clear()
        {
            IsActive = false;
            IsValid = false;
            RemainingUses = 0;
        }
    }
}