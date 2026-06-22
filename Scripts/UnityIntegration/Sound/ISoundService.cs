namespace Quoridor
{
    public interface ISoundService
    {
        void PlayBgm(BgmId bgmId, float fadeSeconds = 0f);
        void StopBgm(float fadeSeconds = 0f);
        void PlaySe(SeId seId, float volumeScale = 1f);

        void SetMasterVolume(float volume);
        void SetBgmVolume(float volume);
        void SetSeVolume(float volume);

        void SetMute(bool isMuted);
    }
}
