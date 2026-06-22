using UnityEngine;

namespace Quoridor
{
    public sealed class PlayerSkillButtonPresenter
    {
        private readonly PlayerId _playerId;
        private readonly SkillButtonSetView _set;
        private readonly SkillButtonViewModel[] _buttonModels;

        private readonly InteractionStateStore _interactionStateStore;
        private readonly InputStateStore _inputStateStore;
        private readonly SkillSelectionStore _skillSelectionStore;

        public PlayerSkillButtonPresenter(
            PlayerId playerId,
            SkillButtonSetView set,
            SkillButtonView[] buttonViews,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore,
            SkillSelectionStore skillSelectionStore
        )
        {
            _playerId = playerId;
            _set = set;
            _buttonModels = CreateAndBindModels(buttonViews);

            _interactionStateStore = interactionStateStore;
            _inputStateStore = inputStateStore;
            _skillSelectionStore = skillSelectionStore;

            Refresh();
        }

        private static SkillButtonViewModel[] CreateAndBindModels(
            SkillButtonView[] buttonViews
        )
        {
            int length = buttonViews.Length;
            var models = new SkillButtonViewModel[length];

            for (int i = 0; i < length; i++)
            {
                models[i] = new SkillButtonViewModel();
                buttonViews[i].BindViewModel(models[i]);
            }

            return models;
        }

        public void Refresh()
        {
            for (int i = 0; i < _buttonModels.Length; i++)
            {
                SkillSlotId skillSlotId = SkillButtonSlotMapper.ToSkillSlotId(
                    i
                );

                InteractionState interactionState =
                    _interactionStateStore.GetSkillState(_playerId, skillSlotId);

                ApplyToModel(
                    _buttonModels[i],
                    skillSlotId,
                    interactionState
                );
            }
        }

        public void Dispose()
        {
            if (_set != null)
            {
                Object.Destroy(_set.gameObject);
            }
        }

        private void ApplyToModel(
            SkillButtonViewModel model,
            SkillSlotId skillSlotId,
            InteractionState interactionState
        )
        {
            model.IsVisible = interactionState.IsActive;
            model.IsValid = interactionState.IsValid;
            model.IsDimmed = !interactionState.IsValid;
            model.RemainingUses = interactionState.RemainingUses;

            model.IsSelected = IsSelectedSkillButton(
                skillSlotId,
                interactionState
            );

            model.IsHovered = IsHoveredSkillButton(skillSlotId);
            model.IsPressed = IsPressedSkillButton(skillSlotId);
        }

        private bool IsSelectedSkillButton(
            SkillSlotId skillSlotId,
            InteractionState interactionState
        )
        {
            if (!interactionState.IsActive) return false;
            if (!interactionState.IsValid) return false;

            return _skillSelectionStore.SelectedSkillSlotId == skillSlotId;
        }

        private bool IsHoveredSkillButton(SkillSlotId skillSlotId)
        {
            return IsSameSkillButton(
                _inputStateStore.HoveredTarget,
                skillSlotId
            );
        }

        private bool IsPressedSkillButton(SkillSlotId skillSlotId)
        {
            return IsSameSkillButton(
                _inputStateStore.PressedTarget,
                skillSlotId
            );
        }

        private bool IsSameSkillButton(
            InputTarget target,
            SkillSlotId skillSlotId
        )
        {
            if (target == null) return false;
            if (target.Kind != InputTargetKind.SkillButton)
            {
                return false;
            }
            return target.SkillSlotId == skillSlotId
                   && target.PlayerId == _playerId;
        }
    }
}
