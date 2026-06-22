namespace Quoridor
{
    /// <summary>
    /// CPU Strategy が意思決定に必要とするターン情報。
    /// </summary>
    public readonly struct CpuAgentDecisionContext
    {
        public MatchState State { get; }
        public PlayerId PlayerId { get; }
        public string Issuer { get; }

        public CpuAgentDecisionContext(MatchState state, PlayerId playerId, string issuer)
        {
            State = Guard.ThrowIfNull(state, nameof(state));
            PlayerId = playerId;
            Issuer = string.IsNullOrEmpty(issuer) ? MatchCommandIssuers.CpuAgent : issuer;
        }
    }
}
