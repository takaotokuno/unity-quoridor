namespace Quoridor
{
    public sealed class MatchInputCommandDispatcher
    {
        private readonly MatchState _state;
        private readonly IMatchCommandPort _commandPort;
        private readonly ISkillDefinitionRegistry _skillRegistry;
        private readonly SkillSelectionController _skillSelectionController;
        private readonly MatchInputRejectionDispatcher _rejectionDispatcher;

        public MatchInputCommandDispatcher(
            MatchState state,
            IMatchCommandPort commandPort,
            ISkillDefinitionRegistry skillRegistry,
            SkillSelectionController skillSelectionController,
            MatchInputRejectionDispatcher rejectionDispatcher
        )
        {
            _state = state;
            _commandPort = commandPort;
            _skillRegistry = skillRegistry;
            _skillSelectionController = skillSelectionController;
            _rejectionDispatcher = rejectionDispatcher;
        }

        public void Dispatch(InputTarget target)
        {
            if (_state.CurrentPlayer.Runtime.IsAuto)
            {
                _skillSelectionController.Clear(_state.CurrentPlayerId);
                _rejectionDispatcher.Reject(target, InputIntent.Released, "current player is auto");
                return;
            }

            switch (target.Kind)
            {
                case InputTargetKind.Tile:
                    DispatchBoardTargetCommand(target, BuiltInSkillSlotIds.MovePawn);
                    break;

                case InputTargetKind.Wall:
                    DispatchBoardTargetCommand(target, BuiltInSkillSlotIds.PlaceWall);
                    break;

                case InputTargetKind.SkillButton:
                    DispatchSkillButtonInput(target);
                    break;

                case InputTargetKind.Button:
                    DispatchButtonCommand(target);
                    break;
            }
        }

        private void DispatchBoardTargetCommand(InputTarget target, SkillSlotId normalSkillSlotId)
        {
            SkillSlotId skillSlotId = _skillSelectionController.SelectedSkillSlotId ?? normalSkillSlotId;

            _skillSelectionController.Clear(_state.CurrentPlayerId);
            _commandPort.DispatchCommand(new UseSkillCommand(
                _state.CurrentPlayerId,
                skillSlotId,
                target.Position,
                MatchCommandIssuers.InputPort
            ));
        }

        private void DispatchSkillButtonInput(InputTarget target)
        {
            if (target.PlayerId != _state.CurrentPlayerId)
            {
                _rejectionDispatcher.Reject(target, InputIntent.Released, "not current player");
                return;
            }

            SkillDefinition definition = FindSkillDefinition(target);

            switch (definition.Type)
            {
                case SkillActivationType.Immediate:
                    DispatchImmediateSkillCommand(target);
                    break;

                case SkillActivationType.BoardTarget:
                    _skillSelectionController.Toggle(target);
                    break;
            }
        }

        private SkillDefinition FindSkillDefinition(InputTarget target)
        {
            PlayerState player = _state.GetPlayer(target.PlayerId);
            SkillId skillId = player.GetSkill(target.SkillSlotId).SkillId;
            return _skillRegistry.Find(skillId);
        }

        private void DispatchImmediateSkillCommand(InputTarget target)
        {
            _skillSelectionController.Clear(target.PlayerId);
            _commandPort.DispatchCommand(new UseSkillCommand(
                target.PlayerId,
                target.SkillSlotId,
                null,
                MatchCommandIssuers.InputPort
            ));
        }

        private void DispatchButtonCommand(InputTarget target)
        {
            switch (target.ButtonId)
            {
                case ButtonId.Resign:
                    _commandPort.DispatchCommand(new ResignCommand(
                        _state.CurrentPlayerId,
                        MatchCommandIssuers.InputPort
                    ));
                    break;

                case ButtonId.Skip:
                    _commandPort.DispatchCommand(new SkipCommand(
                        _state.CurrentPlayerId,
                        MatchCommandIssuers.InputPort
                    ));
                    break;
            }
        }
    }
}
