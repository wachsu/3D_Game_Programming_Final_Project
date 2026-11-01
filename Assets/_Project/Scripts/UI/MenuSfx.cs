using UnityEngine;

namespace Game.UI
{
    /// <summary>Generates short UI blips in code, so the menu needs no audio assets.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class MenuSfx : MonoBehaviour
    {
        public static MenuSfx Instance { get; private set; }

        private AudioSource _source;
        private AudioClip _hover;
        private AudioClip _click;
        private AudioClip _back;

        private void Awake()
        {
            Instance = this;
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _hover = MakeTone("hover", 880f, 0.05f, 0.25f);
            _click = MakeTone("click", 660f, 0.12f, 0.4f, 990f);
            _back = MakeTone("back", 440f, 0.12f, 0.4f, 330f);
        }

        public void PlayHover() => _source.PlayOneShot(_hover);
        public void PlayClick() => _source.PlayOneShot(_click);
        public void PlayBack() => _source.PlayOneShot(_back);

        private static AudioClip MakeTone(string clipName, float startHz, float length, float volume, float endHz = -1f)
        {
            const int sampleRate = 44100;
            if (endHz < 0f) endHz = startHz;
            int count = Mathf.CeilToInt(sampleRate * length);
            var data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float hz = Mathf.Lerp(startHz, endHz, t);
                phase += 2f * Mathf.PI * hz / sampleRate;
                float envelope = Mathf.Sin(Mathf.PI * t);
                data[i] = Mathf.Sin(phase) * envelope * volume;
            }
            var clip = AudioClip.Create(clipName, count, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
