using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class TheaterShowCycle : MonoBehaviour
    {
        [SerializeField] private float litSeconds = 30f;
        [SerializeField] private float darkSeconds = 45f;
        [SerializeField] private float fadeSeconds = 2.5f;
        [SerializeField] private float startOffset = 0f;
        [SerializeField] private string houseLightPrefix = "HouseLight_";

        private readonly List<Light> _houseLights = new List<Light>();
        private readonly List<float> _baseIntensities = new List<float>();

        public bool IsDark { get; private set; }
        public float Brightness { get; private set; } = 1f;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (!child.name.StartsWith(houseLightPrefix)) continue;
                foreach (var l in child.GetComponentsInChildren<Light>(true))
                {
                    _houseLights.Add(l);
                    _baseIntensities.Add(l.intensity);
                }
            }
        }

        private void Update()
        {
            float cycle = Mathf.Max(0.01f, litSeconds + darkSeconds);
            float t = Mathf.Repeat(Time.time + startOffset, cycle);
            float fade = Mathf.Max(0.01f, fadeSeconds);

            if (t < litSeconds)
                Brightness = Mathf.Clamp01(t / fade);
            else
                Brightness = Mathf.Clamp01(1f - (t - litSeconds) / fade);

            IsDark = t >= litSeconds;

            for (int i = 0; i < _houseLights.Count; i++)
            {
                if (_houseLights[i] == null) continue;
                _houseLights[i].intensity = _baseIntensities[i] * Brightness;
                _houseLights[i].enabled = Brightness > 0.001f;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _houseLights.Count; i++)
            {
                if (_houseLights[i] == null) continue;
                _houseLights[i].intensity = _baseIntensities[i];
                _houseLights[i].enabled = true;
            }
        }
    }
}
