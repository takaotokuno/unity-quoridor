using System.Collections;
using UnityEngine;

namespace Quoridor
{
    public sealed class BackgroundEffectManager : MonoBehaviour, IBackgroundEffectService
    {
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private BackgroundEffectCatalog catalog;

        private static readonly int BaseColorAId = Shader.PropertyToID("_BaseColorA");
        private static readonly int BaseColorBId = Shader.PropertyToID("_BaseColorB");
        private static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");
        private static readonly int DistortionStrengthId = Shader.PropertyToID("_DistortionStrength");
        private static readonly int FlowSpeedId = Shader.PropertyToID("_FlowSpeed");
        private static readonly int EmissionStrengthId = Shader.PropertyToID("_EmissionStrength");
        private static readonly int AlphaId = Shader.PropertyToID("_Alpha");
        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashStrengthId = Shader.PropertyToID("_FlashStrength");

        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _transitionCoroutine;
        private BackgroundEffectState _currentState;
        
        void Awake()
        {
            _propertyBlock = new();
        }

        public void ApplyPreset(BackgroundEffectPresetId presetId)
        {
            if (!catalog.TryGet(presetId, out var state))
            {
                Debug.LogWarning($"Preset not found: {presetId}");
                return;
            }

            _currentState = state;
            ApplyStateImmediate(_currentState);
        }

        public void SetIntensity(float intensity)
        {
            _currentState.DistortionStrength *= intensity;
            _currentState.EmissionStrength *= intensity;
            ApplyStateImmediate(_currentState);
        }

        public void Flash(Color color, float duration)
        {
            StartCoroutine(FlashRoutine(color, duration));
        }

        public void TransitionTo(BackgroundEffectState targetState, float duration)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(TransitionRoutine(_currentState, targetState, duration));
        }

        public void ResetToDefault(float duration = 0f)
        {
            if (!catalog.TryGet(BackgroundEffectPresetId.Default, out var state))
            {
                return;
            }

            if (duration <= 0f)
            {
                _currentState = state;
                ApplyStateImmediate(_currentState);
            }
            else
            {
                TransitionTo(state, duration);
            }
        }

        private IEnumerator TransitionRoutine(BackgroundEffectState from, BackgroundEffectState to, float duration)
        {
            if (duration <= 0f)
            {
                _currentState = to;
                ApplyStateImmediate(_currentState);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                _currentState = LerpState(from, to, t);
                ApplyStateImmediate(_currentState);

                yield return null;
            }

            _currentState = to;
            ApplyStateImmediate(_currentState);
            _transitionCoroutine = null;
        }

        private IEnumerator FlashRoutine(Color color, float duration)
        {
            float half = duration * 0.5f;
            float elapsed = 0f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / half);
                SetFlash(color, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = 1f - Mathf.Clamp01(elapsed / half);
                SetFlash(color, t);
                yield return null;
            }

            SetFlash(color, 0f);
        }

        private void SetFlash(Color color, float strength)
        {
            foreach (var renderer in targetRenderers)
            {
                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(FlashColorId, color);
                _propertyBlock.SetFloat(FlashStrengthId, strength);
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void ApplyStateImmediate(BackgroundEffectState state)
        {
            foreach (var renderer in targetRenderers)
            {
                renderer.GetPropertyBlock(_propertyBlock);

                _propertyBlock.SetColor(BaseColorAId, state.BaseColorA);
                _propertyBlock.SetColor(BaseColorBId, state.BaseColorB);
                _propertyBlock.SetFloat(NoiseScaleId, state.NoiseScale);
                _propertyBlock.SetFloat(DistortionStrengthId, state.DistortionStrength);
                _propertyBlock.SetFloat(FlowSpeedId, state.FlowSpeed);
                _propertyBlock.SetFloat(EmissionStrengthId, state.EmissionStrength);
                _propertyBlock.SetFloat(AlphaId, state.Alpha);

                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private static BackgroundEffectState LerpState(BackgroundEffectState a, BackgroundEffectState b, float t)
        {
            return new BackgroundEffectState
            {
                BaseColorA = Color.Lerp(a.BaseColorA, b.BaseColorA, t),
                BaseColorB = Color.Lerp(a.BaseColorB, b.BaseColorB, t),
                NoiseScale = Mathf.Lerp(a.NoiseScale, b.NoiseScale, t),
                DistortionStrength = Mathf.Lerp(a.DistortionStrength, b.DistortionStrength, t),
                FlowSpeed = Mathf.Lerp(a.FlowSpeed, b.FlowSpeed, t),
                EmissionStrength = Mathf.Lerp(a.EmissionStrength, b.EmissionStrength, t),
                Alpha = Mathf.Lerp(a.Alpha, b.Alpha, t),
            };
        }
    }
}
