using UnityEngine;

namespace Game.UI
{
    /// <summary>Persists player options in PlayerPrefs and applies them.</summary>
    public static class GameSettings
    {
        private const string VolumeKey = "settings.volume";
        private const string MuteKey = "settings.mute";
        private const string FullscreenKey = "settings.fullscreen";

        public static float Volume
        {
            get => PlayerPrefs.GetFloat(VolumeKey, 0.8f);
            set { PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value)); Apply(); }
        }

        public static bool Mute
        {
            get => PlayerPrefs.GetInt(MuteKey, 0) == 1;
            set { PlayerPrefs.SetInt(MuteKey, value ? 1 : 0); Apply(); }
        }

        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
            set { PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0); Apply(); }
        }

        public static void Apply()
        {
            AudioListener.volume = Mute ? 0f : Volume;
            Screen.fullScreen = Fullscreen;
            PlayerPrefs.Save();
        }
    }
}
