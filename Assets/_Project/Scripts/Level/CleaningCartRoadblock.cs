using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Game.Level
{
    public class CleaningCartRoadblock : MonoBehaviour
    {
        public enum SwitchOrder { RandomNoRepeat, Sequential }

        [Header("Switching")]
        [SerializeField] private SwitchOrder order = SwitchOrder.RandomNoRepeat;
        [SerializeField] private int startIndex = 0;
        [SerializeField] private bool randomStart = true;
        [SerializeField] private bool keyboardTest = false;

        [Header("Aisle Measurement")]
        [SerializeField] private LayerMask measureMask = (1 << 0) | (1 << 8);
        [SerializeField] private float maxHalfWidth = 5f;
        [SerializeField] private float ignoreSmallerThan = 0.4f;

        [Header("Barrier (invisible)")]
        [SerializeField] private float barrierHeight = 2.4f;
        [SerializeField] private float barrierThickness = 0.4f;

        [Header("Puddle")]
        [SerializeField] private Material puddleMaterial;
        [SerializeField] private float puddleLength = 2.2f;
        [SerializeField, Range(0.5f, 1f)] private float puddleWidthRatio = 0.92f;

        public int CurrentIndex { get; private set; } = -1;
        public event Action<Transform> Moved;

        private readonly List<Transform> _points = new List<Transform>();
        private readonly RaycastHit[] _hits = new RaycastHit[16];

        public struct AisleInfo { public float CenterOffset, Width, FloorY; public bool LeftHit, RightHit; }

        private void Awake()
        {
            _points.Clear();
            foreach (Transform child in transform)
            {
                _points.Add(child);
                child.gameObject.SetActive(true);
            }

            Physics.SyncTransforms();
            foreach (var p in _points)
                BuildBlock(p, Measure(p));

            int first = randomStart && _points.Count > 0 ? Random.Range(0, _points.Count) : Mathf.Clamp(startIndex, 0, Mathf.Max(0, _points.Count - 1));
            Activate(first);
        }

        private void OnEnable() => RestroomArrivalTrigger.PlayerArrived += OnPlayerArrived;
        private void OnDisable() => RestroomArrivalTrigger.PlayerArrived -= OnPlayerArrived;

        private void Update()
        {
            if (keyboardTest && Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
                MoveToNext();
        }

        private void OnPlayerArrived(RestroomArrivalTrigger trigger)
        {
            if (trigger.gameObject.scene == gameObject.scene)
                MoveToNext();
        }

        public void MoveToNext()
        {
            if (_points.Count <= 1)
                return;

            int next;
            if (order == SwitchOrder.Sequential)
                next = (CurrentIndex + 1) % _points.Count;
            else
            {
                next = Random.Range(0, _points.Count - 1);
                if (next >= CurrentIndex) next++;
            }
            Activate(next);
        }

        public void Activate(int index)
        {
            if (index < 0 || index >= _points.Count)
                return;
            for (int i = 0; i < _points.Count; i++)
                _points[i].gameObject.SetActive(i == index);
            CurrentIndex = index;
            Physics.SyncTransforms();
            Moved?.Invoke(_points[index]);
        }

        public AisleInfo Measure(Transform point)
        {
            float floorY = FloorOf(point);
            Vector3 center = new Vector3(point.position.x, floorY, point.position.z);
            Vector3 across = point.forward; across.y = 0f; across.Normalize();

            float left = maxHalfWidth, right = maxHalfWidth;
            bool leftHit = false, rightHit = false;
            foreach (float h in new[] { 0.45f, 1.0f, 1.6f })
            {
                Vector3 o = center + Vector3.up * h;
                if (CastFree(o, across, out float d1) && d1 < right) { right = d1; rightHit = true; }
                if (CastFree(o, -across, out float d2) && d2 < left) { left = d2; leftHit = true; }
            }
            return new AisleInfo
            {
                CenterOffset = (right - left) * 0.5f,
                Width = left + right,
                FloorY = floorY,
                LeftHit = leftHit,
                RightHit = rightHit,
            };
        }

        private bool CastFree(Vector3 origin, Vector3 dir, out float distance)
        {
            distance = maxHalfWidth;
            int n = Physics.SphereCastNonAlloc(origin, 0.2f, dir, _hits, maxHalfWidth, measureMask, QueryTriggerInteraction.Ignore);
            bool found = false;
            for (int i = 0; i < n; i++)
            {
                var hit = _hits[i];
                if (hit.distance <= 0f) continue;
                var col = hit.collider;
                if (col.transform.IsChildOf(transform)) continue;
                if (col.CompareTag("Player")) continue;
                var rb = col.attachedRigidbody;
                if (rb != null && !rb.isKinematic) continue;
                Vector3 s = col.bounds.size;
                if (Mathf.Max(s.x, Mathf.Max(s.y, s.z)) < ignoreSmallerThan) continue;
                if (hit.distance + 0.2f < distance) { distance = hit.distance + 0.2f; found = true; }
            }
            return found;
        }

        private static float FloorOf(Transform point)
        {
            float y = float.MaxValue;
            foreach (var r in point.GetComponentsInChildren<Renderer>(true))
            {
                if (r.name.StartsWith("Roadblock_")) continue;
                y = Mathf.Min(y, r.bounds.min.y);
            }
            return y == float.MaxValue ? point.position.y : y;
        }

        private void BuildBlock(Transform point, AisleInfo aisle)
        {
            Vector3 along = point.right; along.y = 0f; along.Normalize();
            Vector3 across = point.forward; across.y = 0f; across.Normalize();
            Vector3 center = new Vector3(point.position.x, aisle.FloorY, point.position.z) + across * aisle.CenterOffset;
            Quaternion rot = Quaternion.LookRotation(across, Vector3.up);

            var barrier = new GameObject("Roadblock_Barrier");
            barrier.layer = 2;
            barrier.transform.SetParent(point, true);
            barrier.transform.SetPositionAndRotation(center + Vector3.up * barrierHeight * 0.5f, rot);
            var box = barrier.AddComponent<BoxCollider>();
            box.size = new Vector3(barrierThickness, barrierHeight, aisle.Width);

            var puddle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            puddle.name = "Roadblock_Puddle";
            Destroy(puddle.GetComponent<Collider>());
            puddle.transform.SetParent(point, true);
            puddle.transform.SetPositionAndRotation(center + Vector3.up * 0.006f, rot);
            puddle.transform.localScale = Vector3.one;
            puddle.transform.localScale = new Vector3(puddleLength, 0.004f, aisle.Width * puddleWidthRatio);
            var r = puddle.GetComponent<Renderer>();
            r.sharedMaterial = puddleMaterial != null ? puddleMaterial : DefaultPuddleMaterial();
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static Material _defaultPuddle;
        private static Material DefaultPuddleMaterial()
        {
            if (_defaultPuddle != null) return _defaultPuddle;
            _defaultPuddle = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Puddle (runtime)" };
            _defaultPuddle.SetColor("_BaseColor", new Color(0.32f, 0.42f, 0.52f));
            _defaultPuddle.SetFloat("_Smoothness", 0.97f);
            _defaultPuddle.SetFloat("_Metallic", 0.1f);
            return _defaultPuddle;
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;
            Physics.SyncTransforms();
            foreach (Transform p in transform)
            {
                var a = Measure(p);
                Vector3 across = p.forward; across.y = 0f; across.Normalize();
                Vector3 c = new Vector3(p.position.x, a.FloorY, p.position.z) + across * a.CenterOffset + Vector3.up * barrierHeight * 0.5f;
                Gizmos.color = a.LeftHit && a.RightHit ? new Color(0.2f, 1f, 0.3f, 0.6f) : new Color(1f, 0.85f, 0.1f, 0.6f);
                Gizmos.matrix = Matrix4x4.TRS(c, Quaternion.LookRotation(across, Vector3.up), Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(barrierThickness, barrierHeight, a.Width));
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
