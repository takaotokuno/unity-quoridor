using UnityEngine;

namespace Quoridor
{
    public interface IBackgroundEffectService
    {
        void ApplyPreset(BackgroundEffectPresetId presetId);
        void SetIntensity(float intensity);
        void Flash(Color color, float duration);
        void TransitionTo(BackgroundEffectState state, float duration);
        void ResetToDefault(float duration = 0f);
    }   
}
