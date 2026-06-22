using System.Collections.Generic;

namespace Quoridor
{
    public sealed class InteractionStateStore
    {
        private readonly MatchState _state;
        private readonly InteractionState[,] _boardStates;
        private readonly Dictionary<PlayerId, Dictionary<SkillSlotId, InteractionState>> _skillButtonStates;
        private readonly Dictionary<ButtonId, InteractionState> _buttonStates;
        private readonly InteractionStateCalculator _calculator;
        private readonly SkillSelectionStore _skillSelectionStore;
        private PlayerId CurrentPlayerId => _state.CurrentPlayerId;

        public InteractionStateStore(
            MatchState state,
            InteractionStateCalculator calculator,
            SkillSelectionStore skillSelectionStore
        )
        {
            _state = state;
            _calculator = calculator;
            _skillSelectionStore = skillSelectionStore;

            var grid = state.Board.Grid;
            int height = grid.Height;
            int width = grid.Width;

            _boardStates = new InteractionState[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _boardStates[y, x] = new InteractionState();
                }
            }

            _skillButtonStates = new Dictionary<PlayerId, Dictionary<SkillSlotId, InteractionState>>();
            InitializeSkillButtonStates(state);

            _buttonStates = new Dictionary<ButtonId, InteractionState>();
            InitializeButtonStates();

            RefreshBoard();
            RefreshSkillButtons();
            RefreshButtons();
        }

        private void InitializeSkillButtonStates(MatchState state)
        {
            foreach (var player in state.Players)
            {
                var statesBySlot = new Dictionary<SkillSlotId, InteractionState>();

                for (int i = 0; i < player.Skills.Count; i++)
                {
                    SkillSlotId skillSlotId = new(i + 1);
                    statesBySlot[skillSlotId] = new InteractionState();
                }

                _skillButtonStates[player.PlayerId] = statesBySlot;
            }
        }

        private void InitializeButtonStates()
        {
            _buttonStates[ButtonId.Resign] = new InteractionState();
            _buttonStates[ButtonId.Skip] = new InteractionState();
        }

        public InteractionState GetTargetState(InputTarget target)
        {
            switch (target.Kind)
            {
                case InputTargetKind.Tile:
                case InputTargetKind.Wall:
                    return GetBoardState(target.Position.Value);

                case InputTargetKind.SkillButton:
                    return GetSkillState(
                        target.PlayerId,
                        target.SkillSlotId
                    );

                case InputTargetKind.Button:
                    return GetButtonState(target.ButtonId.Value);

                default:
                    throw new System.InvalidOperationException(
                        $"Unsupported InputTargetKind: {target.Kind}"
                    );
            }
        }

        public IReadOnlyList<Position> GetPreviewPositions(Position position)
        {
            // 選択中のスキル
            // ホバー中の対象
            // スキル定義
            // 合法判定
            // などを使って、表示すべき範囲を渡す
            return new List<Position>();
        }

        public InteractionState GetBoardState(Position position)
        {
            return _boardStates[position.Y, position.X];
        }

        public InteractionState GetSkillState(PlayerId playerId, SkillSlotId skillSlotId)
        {
            return _skillButtonStates[playerId][skillSlotId];
        }

        public InteractionState GetButtonState(ButtonId buttonId)
        {
            return _buttonStates[buttonId];
        }

        public void Refresh()
        {
            RefreshBoard();
            RefreshSkillButtons();
            RefreshButtons();
        }

        private void RefreshBoard()
        {
            List<SkillId> skillIds = new();
            var selectedSkillSlotId = _skillSelectionStore.SelectedSkillSlotId;

            if (selectedSkillSlotId == null)
            {
                skillIds.AddRange(FilterCanUseSkillIds(new List<SkillSlotId>
                {
                    BuiltInSkillSlotIds.MovePawn,
                    BuiltInSkillSlotIds.PlaceWall
                }));
            }
            else
            {
                skillIds.AddRange(FilterCanUseSkillIds(new List<SkillSlotId>
                {
                    selectedSkillSlotId
                }));
            }

            _calculator.CalculateForBoard(
                _boardStates,
                _state,
                CurrentPlayerId,
                skillIds
            );
        }

        private IReadOnlyList<SkillId> FilterCanUseSkillIds(IReadOnlyList<SkillSlotId> slotIds)
        {
            List<SkillId> skillIds = new();
            PlayerState player = _state.CurrentPlayer;

            foreach (SkillSlotId slotId in slotIds)
            {
                if (_calculator.CanUseSkill(_state, CurrentPlayerId, slotId))
                {
                    SkillId selectedSkillId = player.GetSkill(slotId).SkillId;
                    skillIds.Add(selectedSkillId);
                }   
            }

            return skillIds;
        }

        public void RefreshSkillButtons()
        {
            _calculator.CalculateForSkillButton(
                _skillButtonStates,
                _state
            );
        }

        public void RefreshButtons()
        {
            var resign = _buttonStates[ButtonId.Resign];
            resign.IsActive = _state.Phase == MatchPhase.InProgress;
            resign.IsValid  = _state.Phase == MatchPhase.InProgress;

            var skip = _buttonStates[ButtonId.Skip];
            // Checkmate 未実装のため、今は常に非表示・無効
            skip.IsActive = false;
            skip.IsValid  = false;
        }
    }
}