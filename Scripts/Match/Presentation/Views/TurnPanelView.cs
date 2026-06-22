using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Quoridor
{
    public sealed class TurnPanelView : ViewBase
    {
        [Header("Panel Elements")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private TMP_Text _turnCountText;

        [Header("Text")]
        [SerializeField] private string _label = "Turn";
        [SerializeField] private int _initialTurnCount = 1;

        protected override void OnInitialize()
        {
            if (_backgroundImage == null)
            {
                _backgroundImage = GetComponent<Image>();
            }

            if (_labelText != null)
            {
                _labelText.text = _label;
            }

            if (_turnCountText != null)
            {
                _turnCountText.text = _initialTurnCount.ToString();
            }
        }

        public void UpdateTurnCount(int currentTurn)
        {
            if (_turnCountText == null)
            {
                return;
            }

            _turnCountText.text = currentTurn.ToString();
        }

        public void SetLabel(string label)
        {
            _label = label;

            if (_labelText == null)
            {
                return;
            }

            _labelText.text = _label;
        }
    }
}