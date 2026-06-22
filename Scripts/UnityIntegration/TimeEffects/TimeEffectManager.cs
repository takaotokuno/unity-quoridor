using System.Collections;
using UnityEngine;

namespace Quoridor
{
    public sealed class TimeEffectManager : MonoBehaviour, ITimeEffectService
    {
        public static TimeEffectManager Instance => _instance;
        private static TimeEffectManager _instance;
        private Coroutine _currentRoutine;
        private float _defaultFixedDeltaTime;

        void Awake()
        {
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
        }
        
        public void ApplyHitStop(float duration, float timeScale = 0f)
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
            }

            _currentRoutine = StartCoroutine(HitStopRoutine(duration, timeScale));
        }

        private IEnumerator HitStopRoutine(float duration, float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * timeScale;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
        }
    }
}
