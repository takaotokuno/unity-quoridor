using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Quoridor
{
    public sealed class SkillButtonView : ViewBase, IUserInteractable,
        IPointerEnterHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _remainText;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _highlightColor = new Color(0.8f, 1f, 0.8f);
        [SerializeField] private Color _emphasizeColor = new Color(1f, 0.95f, 0.4f);
        [SerializeField] private Color _dimColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color _pressedColor = new Color(0.75f, 0.85f, 1f);
        [SerializeField] private Color _invalidColor = new Color(1f, 0.35f, 0.35f);

        [SerializeField] private Vector3 _highlightScale = new Vector3(1.05f, 1.05f, 1.05f);
        [SerializeField] private Vector3 _emphasizeScale = new Vector3(1.12f, 1.12f, 1.12f);
        [SerializeField] private Vector3 _pressedScale = new Vector3(0.96f, 0.96f, 0.96f);
        private MatchInputPort _inputPort;
        private InputTarget _target;
        public SkillButtonViewModel ViewModel { get; private set; }

        public void Initialize(
            PlayerId playerId,
            SkillSlotId skillSlotId
        )
        {
            _target = InputTarget.SkillButton(playerId, skillSlotId);
        }

        public void BindViewDefinition(SkillViewEntry entry)
        {
            if (entry == null) return;

            _iconImage.sprite = entry.Icon;
            _nameText.text = entry.DisplayName;
        }

        public void BindInputPort(MatchInputPort inputPort)
        {
            _inputPort = inputPort;
        }

        public void BindViewModel(SkillButtonViewModel viewModel)
        {
            if (ViewModel != null)
            {
                ViewModel.Changed -= OnViewModelChangedInternal;
            }

            ViewModel = viewModel;

            if (ViewModel != null)
            {
                ViewModel.Changed += OnViewModelChangedInternal;
                OnViewModelChangedInternal();
            }
        }

        private void OnViewModelChangedInternal()
        {
            if (ViewModel == null) return;
            ApplyViewModel();
        }

        private void ApplyViewModel()
        {
            if (ViewModel == null) return;

            if (ViewModel.IsVisible)
            {
                Show();
            }
            else
            {
                Hide();
                return;
            }

            if (ViewModel.IsDimmed)
            {
                Dim();   
            }
            else if (ViewModel.IsPressed && ViewModel.IsValid)
            {
                Press();
            }
            else if (ViewModel.IsSelected && ViewModel.IsValid)
            {
                Emphasize();   
            }
            else if (ViewModel.IsHovered && ViewModel.IsValid)
            {
                Highlight();
            }
            else
            {
                Clear();
            }

            _remainText.text = ViewModel.RemainingUses.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Pressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Released();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOut();
        }

        public void Hovered()
        {
            _inputPort.Handle(_target, InputIntent.Hovered);
        }

        public void Pressed()
        {
            _inputPort.Handle(_target, InputIntent.Pressed);
        }

        public void Released()
        {
            _inputPort.Handle(_target, InputIntent.Released);
        }

        public void MouseOut()
        {
            _inputPort.Handle(_target, InputIntent.MouseOut);
        }

        public void Highlight()
        {
            SetColor(_highlightColor);
            SetScale(_highlightScale);
        }

        public void Emphasize()
        {
            SetColor(_emphasizeColor);
            SetScale(_emphasizeScale);
        }

        public void Dim()
        {
            SetColor(_dimColor);
            SetScale(_initialScale);
        }

        public void Press()
        {
            SetColor(_pressedColor);
            SetScale(_pressedScale);
        }

        public void Clear()
        {
            SetColor(_normalColor);
            SetScale(_initialScale);
        }

        public void PlayInvalidFeedback()
        {
            SetColor(_invalidColor);
            SetScale(_emphasizeScale);
        }

        private void SetColor(Color color)
        {
            _iconImage.color = color;
        }
    }   
}
