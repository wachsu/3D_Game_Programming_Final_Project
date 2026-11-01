using UnityEngine;

namespace Game.UI
{
    /// <summary>Spins, bobs and drifts a backdrop object. Parameters are randomised by the spawner.</summary>
    public class FloatingItem : MonoBehaviour
    {
        public Vector3 SpinSpeed;
        public float BobAmplitude;
        public float BobSpeed;
        public float BobPhase;
        public float RiseSpeed;
        public float WrapHeight;

        private Vector3 _origin;
        private float _risen;

        private void Start() => _origin = transform.position;

        private void Update()
        {
            float dt = Time.deltaTime;
            transform.Rotate(SpinSpeed * dt, Space.Self);

            _risen = Mathf.Repeat(_risen + RiseSpeed * dt, WrapHeight);
            float bob = Mathf.Sin(Time.time * BobSpeed + BobPhase) * BobAmplitude;
            transform.position = _origin + Vector3.up * (bob + _risen - WrapHeight * 0.5f);
        }
    }
}
