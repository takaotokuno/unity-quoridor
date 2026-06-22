using UnityEngine;

namespace Quoridor
{
    public sealed class SoundManager : MonoBehaviour, ISoundService
    {
        [SerializeField] private SoundCatalog catalog;

        private AudioSource _bgmSource;
        private AudioSource _seSource;

        private float _masterVolume = 1f;
        private float _bgmVolume = 1f;
        private float _seVolume = 1f;
        private bool _isMuted = false;

        void Awake()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _seSource = gameObject.AddComponent<AudioSource>();

            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _seSource.loop = false;
            _seSource.playOnAwake = false;

            ApplyVolumes();
        }

        public void PlayBgm(BgmId id, float fadeSeconds = 0f)
        {
            if (catalog == null) return;

            var clip = catalog.FindBgm(id);
            if (clip == null) return;

            if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

            _bgmSource.clip = clip;
            ApplyVolumes();
            _bgmSource.Play();
        }

        public void StopBgm(float fadeSeconds = 0f)
        {
            if (_bgmSource == null) return;

            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        public void PlaySe(SeId id, float fadeSeconds = 0f)
        {
            if (catalog == null) return;

            var clip = catalog.FindSe(id);
            if (clip == null) return;

            ApplyVolumes();
            _seSource.PlayOneShot(clip, GetEffectiveSeVolume());
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
        }

        public void SetBgmVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
        }

        public void SetSeVolume(float volume)
        {
            _seVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (_bgmSource != null)
            {
                _bgmSource.volume = GetEffectiveBgmVolume();
            }

            if (_seSource != null)
            {
                _seSource.volume = GetEffectiveSeVolume();
            }
        }

        private float GetEffectiveBgmVolume()
        {
            if (_isMuted) return 0f;
            return _masterVolume * _bgmVolume;
        }

        private float GetEffectiveSeVolume()
        {
            if (_isMuted) return 0f;
            return _masterVolume * _seVolume;
        }
    }
}
