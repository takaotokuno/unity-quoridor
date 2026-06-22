using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public sealed partial class PresenterFactory
    {
        public SkillButtonPresenter CreateSkillButton()
        {
            IReadOnlyList<SkillId> skillsFirst = _setting.SkillIdsFirst;
            SkillButtonSetView setFirst = layout.SkillButtonSetViewFirst;

            SkillButtonView[] buttonsFirst = CreateSkillButtons(
                PlayerId.FirstPlayer,
                skillsFirst,
                setFirst.transform
            );

            IReadOnlyList<SkillId> skillsSecond = _setting.SkillIdsSecond;
            SkillButtonSetView setSecond = layout.SkillButtonSetViewSecond;

            SkillButtonView[] buttonsSecond = CreateSkillButtons(
                PlayerId.SecondPlayer,
                skillsSecond,
                setSecond.transform
            );

            SkillButtonPresenter presenter = new(
                setFirst,
                buttonsFirst,
                setSecond,
                buttonsSecond,
                _interactionStateStore,
                _inputStateStore,
                _skillSelectionStore
            );

            presenter.SubscribeTo(_eventBus);

            return presenter;
        }

        private SkillButtonView[] CreateSkillButtons(
            PlayerId playerId,
            IReadOnlyList<SkillId> skills,
            Transform parent
        )
        {
            if (skills == null)
            {
                throw new System.ArgumentNullException(nameof(skills));
            }

            if (parent == null)
            {
                throw new System.ArgumentNullException(nameof(parent));
            }

            int buttonCount = SkillButtonSlotMapper.GetSkillButtonCount(
                skills.Count
            );

            SkillButtonView[] buttons = new SkillButtonView[buttonCount];

            for (int buttonIndex = 0;
                 buttonIndex < buttonCount;
                 buttonIndex++)
            {
                SkillSlotId skillSlotId = SkillButtonSlotMapper.ToSkillSlotId(
                    buttonIndex
                );
                SkillId skillId = skills[skillSlotId.ToIndex()];

                SkillButtonView skillButton = CreateSkillButtonView(parent);

                var viewEntry = _skillViewCatalog.Find(skillId);

                skillButton.Initialize(playerId, skillSlotId);
                skillButton.BindInputPort(_inputPort);
                skillButton.BindViewDefinition(viewEntry);
                buttons[buttonIndex] = skillButton;
            }

            return buttons;
        }

        private SkillButtonView CreateSkillButtonView(Transform parent)
        {
            if (presenterSetting.SkillButtonPrefab == null)
            {
                throw new System.InvalidOperationException(
                    $"{nameof(presenterSetting.SkillButtonPrefab)} is not assigned."
                );
            }

            SkillButtonView skillButton = Object.Instantiate(
                presenterSetting.SkillButtonPrefab,
                parent
            );

            RectTransform rectTransform = skillButton.transform as RectTransform;

            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.localRotation = Quaternion.identity;

                // VerticalLayoutGroup が配置を管理するため、
                // 位置は意味を持たせない。
                rectTransform.anchoredPosition3D = Vector3.zero;
            }

            return skillButton;
        }
    }
}
