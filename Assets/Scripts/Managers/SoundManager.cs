using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public enum SfxId
    {
        UiClick,
        PlayerHit,
        EnemyDeath,
        Pickup,
        Portal,
        Potion
    }

    private AudioSource sfxSource;
    private AudioSource bgmSource;
    private bool warnedMissingClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        ApplyVolumeSettings();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ApplyVolumeSettings()
    {
        if (sfxSource != null)
            sfxSource.volume = GameSettings.EffectiveSfxVolume;

        if (bgmSource != null)
            bgmSource.volume = GameSettings.EffectiveBgmVolume;
    }

    public void PlaySfx(SfxId sfxId)
    {
        string resourcePath = GetResourcePath(sfxId);
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);

        if (clip == null)
        {
            if (!warnedMissingClip)
            {
                Debug.Log("SoundManager: Audio clip not found yet. Add WAV/OGG under Assets/Resources/Audio/...");
                warnedMissingClip = true;
            }

            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayBgm(string resourcePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.volume = GameSettings.EffectiveBgmVolume;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    private static string GetResourcePath(SfxId sfxId)
    {
        switch (sfxId)
        {
            case SfxId.UiClick: return "Audio/SFX/ui_click";
            case SfxId.PlayerHit: return "Audio/SFX/player_hit";
            case SfxId.EnemyDeath: return "Audio/SFX/enemy_death";
            case SfxId.Pickup: return "Audio/SFX/pickup";
            case SfxId.Portal: return "Audio/SFX/portal";
            case SfxId.Potion: return "Audio/SFX/potion";
            default: return string.Empty;
        }
    }
}
