using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Quoridor
{
    public sealed class StatusIconView 
        : ViewBase,
          IPointerEnterHandler,
          IPointerExitHandler
    {
        [Header("UI References")]
        [SerializeField] private Image _iconImage;

        [Header("Description")]
        [SerializeField] private GameObject _descriptionRoot;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;

        [Header("Show / Hide Animation")]
        [SerializeField] private float _showHideDuration = 0.18f;
        [SerializeField] private float _showOffsetY = 12f;
        [SerializeField] private float _hideOffsetY = -8f;

        private Coroutine _showHideCoroutine;
        private RectTransform _iconRectTransform;
        private Vector2 _baseIconAnchoredPosition;

        protected override void OnInitialize()
        {
            if (_iconImage != null)
            {
                _iconRectTransform = _iconImage.rectTransform;
                _baseIconAnchoredPosition = _iconRectTransform.anchoredPosition;
                SetIconAlpha(0f);
                _iconImage.enabled = false;
            }

            HideDescription();
        }

        public void BindViewDefinition(StatusViewEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            if (_iconImage != null)
            {
                _iconImage.sprite = entry.Icon;
            }

            if (_nameText != null)
            {
                _nameText.text = entry.DisplayName;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = entry.Description;
            }
        }

        public override void PlayShow()
        {
            if (_iconImage == null)
            {
                return;
            }

            _iconImage.enabled = true;

            StartShowHideAnimation(
                startAlpha: 0f,
                endAlpha: 1f,
                startPos: _baseIconAnchoredPosition + new Vector2(0f, _showOffsetY),
                endPos: _baseIconAnchoredPosition,
                hideIconOnComplete: false
            );
        }

        public override void PlayHide()
        {
            PlayHide(null);
        }

        public void PlayHide(Action onComplete)
        {
            if (_iconImage == null)
            {
                onComplete?.Invoke();
                return;
            }

            HideDescription();

            float currentAlpha = _iconImage.color.a;

            Vector2 currentPos = _iconRectTransform != null
                ? _iconRectTransform.anchoredPosition
                : _baseIconAnchoredPosition;

            StartShowHideAnimation(
                startAlpha: currentAlpha,
                endAlpha: 0f,
                startPos: currentPos,
                endPos: _baseIconAnchoredPosition + new Vector2(0f, _hideOffsetY),
                hideIconOnComplete: true,
                onComplete: onComplete
            );
        }

        public void ShowDescription()
        {
            if (_descriptionRoot == null) return;
            _descriptionRoot.SetActive(true);
        }

        public void HideDescription()
        {
            if (_descriptionRoot != null)
            {
                _descriptionRoot.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowDescription();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideDescription();
        }

        private void StartShowHideAnimation(
            float startAlpha,
            float endAlpha,
            Vector2 startPos,
            Vector2 endPos,
            bool hideIconOnComplete,
            Action onComplete = null
        )
        {
            if (_showHideCoroutine != null)
            {
                StopCoroutine(_showHideCoroutine);
            }

            _showHideCoroutine = StartCoroutine(
                PlayShowHideCoroutine(
                    startAlpha,
                    endAlpha,
                    startPos,
                    endPos,
                    hideIconOnComplete,
                    onComplete
                )
            );
        }

        private IEnumerator PlayShowHideCoroutine(
            float startAlpha,
            float endAlpha,
            Vector2 startPos,
            Vector2 endPos,
            bool hideIconOnComplete,
            Action onComplete
        )
        {
            float elapsed = 0f;

            SetIconAlpha(startAlpha);
            SetIconPosition(startPos);

            while (elapsed < _showHideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _showHideDuration);
                float easedT = 1f - Mathf.Pow(1f - t, 3f);

                SetIconAlpha(Mathf.Lerp(startAlpha, endAlpha, easedT));
                SetIconPosition(Vector2.Lerp(startPos, endPos, easedT));

                yield return null;
            }

            SetIconAlpha(endAlpha);
            SetIconPosition(endPos);

            if (hideIconOnComplete)
            {
                _iconImage.enabled = false;
                SetIconPosition(_baseIconAnchoredPosition);
            }

            _showHideCoroutine = null;
            onComplete?.Invoke();
        }

        private void SetIconAlpha(float alpha)
        {
            if (_iconImage == null) return;
            Color color = _iconImage.color;
            color.a = alpha;
            _iconImage.color = color;
        }

        private void SetIconPosition(Vector2 anchoredPosition)
        {
            if (_iconRectTransform == null) return;
            _iconRectTransform.anchoredPosition = anchoredPosition;
        }
    }
}
