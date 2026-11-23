using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MiniProjects.HierarchicalModel
{
    public class PlaneRideAnimator : MonoBehaviour
    {
        [Header("Joints")]
        [SerializeField] private Transform rotorA;
        [SerializeField] private Transform[] armsB;
        [SerializeField] private Transform[] planesC;
        [SerializeField] private Transform[] propellers;

        [Header("A: Rotor")]
        [SerializeField] private float rotorSpeed = 40f;

        [Header("B: Arms")]
        [SerializeField] private float armSpeed = 18f;
        [SerializeField] private float armUpLimit = -22f;
        [SerializeField] private float armDownLimit = 12f;

        [Header("C: Planes")]
        [SerializeField] private Vector3 planeBaseLocal = new Vector3(0f, -0.35f, 2.35f);
        [SerializeField] private float bobAmplitude = 0.18f;
        [SerializeField] private float bobFrequency = 0.8f;

        [Header("Decoration")]
        [SerializeField] private float propellerSpeed = 900f;

        [Header("Input")]
        [SerializeField] private bool enableKeyControls = true;

        private float _rotorAngle;
        private int _rotorDir = 1;
        private float[] _armAngles;
        private int[] _armDirs;
        private float _bobPhase;
        private int _bobDir = 1;

        public int RotorDirection => _rotorDir;
        public int BobDirection => _bobDir;

        private void Awake()
        {
            int n = armsB != null ? armsB.Length : 0;
            _armAngles = new float[n];
            _armDirs = new int[n];
            for (int i = 0; i < n; i++)
            {
                float t = n > 1 ? (float)i / n : 0f;
                _armAngles[i] = Mathf.Lerp(armUpLimit, armDownLimit, Mathf.PingPong(t * 2f, 1f));
                _armDirs[i] = i % 2 == 0 ? 1 : -1;
            }
        }

        private void Update()
        {
            HandleKeys();

            float dt = Time.deltaTime;

            if (rotorA != null)
            {
                _rotorAngle = Mathf.Repeat(_rotorAngle + _rotorDir * rotorSpeed * dt, 360f);
                rotorA.localRotation = Quaternion.Euler(0f, _rotorAngle, 0f);
            }

            if (armsB != null)
            {
                for (int i = 0; i < armsB.Length; i++)
                {
                    if (armsB[i] == null) continue;
                    _armAngles[i] += _armDirs[i] * armSpeed * dt;
                    if (_armAngles[i] > armDownLimit) { _armAngles[i] = armDownLimit; _armDirs[i] = -1; }
                    if (_armAngles[i] < armUpLimit) { _armAngles[i] = armUpLimit; _armDirs[i] = 1; }
                    armsB[i].localRotation = Quaternion.Euler(_armAngles[i], 0f, 0f);
                }
            }

            _bobPhase += _bobDir * bobFrequency * 2f * Mathf.PI * dt;
            if (planesC != null)
            {
                for (int i = 0; i < planesC.Length; i++)
                {
                    if (planesC[i] == null) continue;
                    float phase = _bobPhase + i * (2f * Mathf.PI / planesC.Length);
                    planesC[i].localPosition = planeBaseLocal + Vector3.up * (bobAmplitude * Mathf.Sin(phase));
                }
            }

            if (propellers != null)
                foreach (var p in propellers)
                    if (p != null) p.Rotate(0f, 0f, propellerSpeed * dt, Space.Self);
        }

        private void HandleKeys()
        {
            if (!enableKeyControls || Keyboard.current == null)
                return;

            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                _rotorDir = -_rotorDir;
                Debug.Log($"[PlaneRide] A rotor direction: {(_rotorDir > 0 ? "counter-clockwise" : "clockwise")}");
            }
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                for (int i = 0; i < _armDirs.Length; i++)
                    _armDirs[i] = -_armDirs[i];
                Debug.Log("[PlaneRide] B arms reversed");
            }
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                _bobDir = -_bobDir;
                Debug.Log($"[PlaneRide] C bob direction: {_bobDir}");
            }
        }

        public void Assign(Transform rotor, Transform[] arms, Transform[] planes, Transform[] props)
        {
            rotorA = rotor;
            armsB = arms;
            planesC = planes;
            propellers = props;
        }
    }
}
