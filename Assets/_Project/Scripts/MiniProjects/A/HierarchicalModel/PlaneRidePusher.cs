using System.Collections.Generic;
using UnityEngine;

namespace Game.MiniProjects.HierarchicalModel
{
    [DefaultExecutionOrder(50)]
    public class PlaneRidePusher : MonoBehaviour
    {
        [SerializeField] private float pushSpeed = 6f;
        [SerializeField] private float pushDuration = 0.35f;
        [SerializeField] private float partSpeedMultiplier = 1.5f;
        [SerializeField] private float skin = 0.03f;
        [SerializeField] private string playerTag = "Player";

        private readonly List<Collider> _parts = new List<Collider>();
        private readonly Dictionary<Collider, Vector3> _lastCenters = new Dictionary<Collider, Vector3>();
        private readonly Collider[] _overlaps = new Collider[32];
        private CharacterController _player;
        private Vector3 _pushVelocity;
        private float _pushTimer;

        private void Start()
        {
            foreach (var c in GetComponentsInChildren<Collider>())
                if (c.enabled && !c.isTrigger)
                    _parts.Add(c);

            var go = GameObject.FindWithTag(playerTag);
            if (go != null)
                _player = go.GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_player == null || !_player.enabled)
                return;

            Physics.SyncTransforms();
            float dt = Mathf.Max(Time.deltaTime, 1e-4f);

            Transform pt = _player.transform;
            float radius = _player.radius * Mathf.Max(pt.lossyScale.x, pt.lossyScale.z);
            float height = Mathf.Max(_player.height * pt.lossyScale.y, radius * 2f);
            Vector3 center = pt.TransformPoint(_player.center);
            Vector3 up = Vector3.up * (height * 0.5f - radius);
            Vector3 p0 = center - up, p1 = center + up;

            Vector3 separation = Vector3.zero;
            bool hit = false;

            int n = Physics.OverlapCapsuleNonAlloc(p0, p1, radius, _overlaps, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < n; i++)
            {
                Collider part = _overlaps[i];
                if (!_parts.Contains(part))
                    continue;

                Vector3 partCenter = part.bounds.center;
                Vector3 partVel = _lastCenters.TryGetValue(part, out var last) ? (partCenter - last) / dt : Vector3.zero;
                partVel.y = 0f;

                Vector3 axisPoint = new Vector3(center.x, Mathf.Clamp(partCenter.y, p0.y, p1.y), center.z);
                Vector3 closest = part.ClosestPoint(axisPoint);
                Vector3 away = axisPoint - closest; away.y = 0f;
                float dist = away.magnitude;
                Vector3 dir = dist > 1e-3f ? away / dist : (partVel.sqrMagnitude > 1e-4f ? partVel.normalized : (center - transform.position).WithY0Normalized());

                float penetration = dist > 1e-3f ? radius - dist + skin : radius + skin;
                if (penetration > 0f)
                    separation += dir * penetration;

                Vector3 pushDir = partVel.sqrMagnitude > 1e-4f ? (partVel.normalized + dir).normalized : dir;
                float speed = Mathf.Max(pushSpeed, partVel.magnitude * partSpeedMultiplier);
                _pushVelocity = pushDir * speed;
                _pushTimer = pushDuration;
                hit = true;
            }

            foreach (var c in _parts)
                _lastCenters[c] = c.bounds.center;

            Vector3 move = separation;
            if (_pushTimer > 0f)
            {
                float k = pushDuration > 0f ? _pushTimer / pushDuration : 0f;
                move += _pushVelocity * k * dt;
                _pushTimer -= dt;
            }
            if (move.sqrMagnitude > 1e-8f)
                _player.Move(move);

            if (hit)
                Debug.DrawRay(center, _pushVelocity * 0.2f, Color.red);
        }
    }

    internal static class VectorExtensions
    {
        public static Vector3 WithY0Normalized(this Vector3 v)
        {
            v.y = 0f;
            return v.sqrMagnitude > 1e-6f ? v.normalized : Vector3.forward;
        }
    }
}
