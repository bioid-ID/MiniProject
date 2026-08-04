public static class GameSettings
{
    private const string MasterVolumeKey = "MiniProject_MasterVolume";
    private const string SfxVolumeKey = "MiniProject_SfxVolume";
    private const string BgmVolumeKey = "MiniProject_BgmVolume";

    public static float MasterVolume
    {
        get => UnityEngine.PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        set
        {
            UnityEngine.PlayerPrefs.SetFloat(MasterVolumeKey, UnityEngine.Mathf.Clamp01(value));
            UnityEngine.PlayerPrefs.Save();
            SoundManager.Instance?.ApplyVolumeSettings();
        }
    }

    public static float SfxVolume
    {
        get => UnityEngine.PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        set
        {
            UnityEngine.PlayerPrefs.SetFloat(SfxVolumeKey, UnityEngine.Mathf.Clamp01(value));
            UnityEngine.PlayerPrefs.Save();
            SoundManager.Instance?.ApplyVolumeSettings();
        }
    }

    public static float BgmVolume
    {
        get => UnityEngine.PlayerPrefs.GetFloat(BgmVolumeKey, 0.6f);
        set
        {
            UnityEngine.PlayerPrefs.SetFloat(BgmVolumeKey, UnityEngine.Mathf.Clamp01(value));
            UnityEngine.PlayerPrefs.Save();
            SoundManager.Instance?.ApplyVolumeSettings();
        }
    }

    public static float EffectiveSfxVolume => MasterVolume * SfxVolume;
    public static float EffectiveBgmVolume => MasterVolume * BgmVolume;
}
