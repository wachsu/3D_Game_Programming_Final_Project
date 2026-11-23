using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Level
{
    public class FloorAmbientDarkener : MonoBehaviour
    {
        [SerializeField] private float minPlayerY = 25f;
        [SerializeField] private float maxPlayerY = 45f;
        [SerializeField, Range(0f, 1f)] private float ambientScale = 0.12f;
        [SerializeField, Range(0f, 1f)] private float reflectionIntensity = 0.15f;
        [SerializeField] private float blendSeconds = 1f;
        [SerializeField] private string playerTag = "Player";

        private Transform _player;
        private SphericalHarmonicsL2 _originalProbe;
        private float _originalReflection;
        private float _weight;
        private bool _captured;

        private void Start()
        {
            _originalProbe = RenderSettings.ambientProbe;
            _originalReflection = RenderSettings.reflectionIntensity;
            _captured = true;
            var go = GameObject.FindWithTag(playerTag);
            if (go != null) _player = go.transform;
        }

        private void Update()
        {
            if (!_captured) return;
            if (_player == null)
            {
                var go = GameObject.FindWithTag(playerTag);
                if (go == null) return;
                _player = go.transform;
            }

            float y = _player.position.y;
            float target = y >= minPlayerY && y <= maxPlayerY ? 1f : 0f;
            _weight = Mathf.MoveTowards(_weight, target, Time.deltaTime / Mathf.Max(0.01f, blendSeconds));
            Apply(_weight);
        }

        private void Apply(float w)
        {
            RenderSettings.ambientProbe = _originalProbe * Mathf.Lerp(1f, ambientScale, w);
            RenderSettings.reflectionIntensity = Mathf.Lerp(_originalReflection, reflectionIntensity, w);
        }

        private void OnDisable()
        {
            if (!_captured) return;
            RenderSettings.ambientProbe = _originalProbe;
            RenderSettings.reflectionIntensity = _originalReflection;
        }
    }
}
