using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Fills the 3D menu backdrop with floating food items generated in a loop,
    /// and sways the camera so the scene feels alive behind the UI.
    /// </summary>
    public class MenuBackdrop : MonoBehaviour
    {
        [Header("Spawning")]
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private int _count = 80;
        [SerializeField] private Vector3 _area = new Vector3(18f, 8f, 10f);
        [SerializeField] private Vector2 _scaleRange = new Vector2(1.2f, 2.6f);
        [SerializeField] private int _seed = 7;

        [Header("Camera sway")]
        [SerializeField] private Transform _cameraRig;
        [SerializeField] private float _swayAngle = 4f;
        [SerializeField] private float _swaySpeed = 0.15f;

        private Quaternion _baseRotation;

        private void Start()
        {
            if (_cameraRig != null) _baseRotation = _cameraRig.rotation;
            Spawn();
        }

        private void Update()
        {
            if (_cameraRig == null) return;
            float yaw = Mathf.Sin(Time.time * _swaySpeed) * _swayAngle;
            float pitch = Mathf.Sin(Time.time * _swaySpeed * 0.7f + 1f) * _swayAngle * 0.4f;
            _cameraRig.rotation = _baseRotation * Quaternion.Euler(pitch, yaw, 0f);
        }

        private void Spawn()
        {
            if (_prefabs == null || _prefabs.Length == 0) return;

            var rng = new System.Random(_seed);
            float Range(float a, float b) => a + (float)rng.NextDouble() * (b - a);

            for (int i = 0; i < _count; i++)
            {
                GameObject prefab = _prefabs[i % _prefabs.Length];
                Vector3 pos = transform.position + new Vector3(
                    Range(-_area.x, _area.x),
                    Range(-_area.y, _area.y) * 0.5f,
                    Range(0f, _area.z));

                var item = Instantiate(prefab, pos, Random.rotationUniform, transform);
                item.name = prefab.name + "_" + i.ToString("00");
                item.transform.localScale = Vector3.one * Range(_scaleRange.x, _scaleRange.y);

                var fx = item.AddComponent<FloatingItem>();
                fx.SpinSpeed = new Vector3(Range(-40f, 40f), Range(-60f, 60f), Range(-40f, 40f));
                fx.BobAmplitude = Range(0.1f, 0.5f);
                fx.BobSpeed = Range(0.6f, 1.6f);
                fx.BobPhase = Range(0f, Mathf.PI * 2f);
                fx.RiseSpeed = Range(0.05f, 0.25f);
                fx.WrapHeight = _area.y * 2f;
            }
        }
    }
}
