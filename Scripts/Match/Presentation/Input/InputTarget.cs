namespace Quoridor
{
    public sealed record InputTarget
    {
        public InputTargetKind Kind { get; }
        public Position? Position { get; }
        public PlayerId PlayerId { get; }
        public SkillSlotId SkillSlotId { get; }
        public ButtonId? ButtonId { get; }

        private InputTarget(
            InputTargetKind kind,
            Position? position,
            PlayerId playerId,
            SkillSlotId skillSlotId,
            ButtonId? buttonId
        )
        {
            Kind = kind;
            Position = position;
            PlayerId = playerId;
            SkillSlotId = skillSlotId;
            ButtonId = buttonId;
        }

        public static InputTarget Tile(Position position)
        {
            return new InputTarget(InputTargetKind.Tile, position, null, null, null);
        }

        public static InputTarget Wall(Position position)
        {
            return new InputTarget(InputTargetKind.Wall, position, null, null, null);   
        }

        public static InputTarget SkillButton(PlayerId playerId, SkillSlotId skillSlotId)
        {
            Guard.ThrowIfNull(playerId, nameof(playerId));
            Guard.ThrowIfNull(skillSlotId, nameof(skillSlotId));

            return new InputTarget(InputTargetKind.SkillButton, null, playerId, skillSlotId, null);   
        }

        public static InputTarget Button(ButtonId buttonId)
        {
            return new InputTarget(InputTargetKind.Button,null, null, null, buttonId);
        }
    }
}