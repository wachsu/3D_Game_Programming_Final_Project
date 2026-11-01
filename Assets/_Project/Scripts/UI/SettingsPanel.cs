using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>Binds the settings controls to <see cref="GameSettings"/>.</summary>
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Toggle _muteToggle;
        [SerializeField] private Toggle _fullscreenToggle;

        private void OnEnable()
        {
            _volumeSlider.SetValueWithoutNotify(GameSettings.Volume);
            _muteToggle.SetIsOnWithoutNotify(GameSettings.Mute);
            _fullscreenToggle.SetIsOnWithoutNotify(GameSettings.Fullscreen);

            _volumeSlider.onValueChanged.AddListener(OnVolume);
            _muteToggle.onValueChanged.AddListener(OnMute);
            _fullscreenToggle.onValueChanged.AddListener(OnFullscreen);
        }

        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveListener(OnVolume);
            _muteToggle.onValueChanged.RemoveListener(OnMute);
            _fullscreenToggle.onValueChanged.RemoveListener(OnFullscreen);
        }

        private static void OnVolume(float v) => GameSettings.Volume = v;
        private static void OnMute(bool on) => GameSettings.Mute = on;
        private static void OnFullscreen(bool on) => GameSettings.Fullscreen = on;
    }
}
