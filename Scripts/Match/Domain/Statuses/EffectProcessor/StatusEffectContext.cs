namespace Quoridor
{
    public sealed record StatusEffectContext
    {
        public PlayerId PlayerId { get; }
        public PlayerState Player { get; }
        public StatusId StatusId { get; }
        public StatusEffectDefinition EffectDefinition { get; }

        public StatusEffectContext(
            PlayerId playerId,
            PlayerState player,
            StatusId statusId,
            StatusEffectDefinition effectDefinition
        )
        {
            PlayerId = playerId;
            Player = player;
            StatusId = statusId;
            EffectDefinition = effectDefinition;
        }
    }
}