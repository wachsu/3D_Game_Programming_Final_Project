using UnityEngine;

namespace Game.Level
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public class CameraObstacleAvoidance : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraFollow follow;

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask = (1 << 0) | (1 << 8) | (1 << 9);
        [SerializeField] private float castRadius = 0.3f;
        [SerializeField] private float wallPadding = 0.05f;
        [SerializeField] private float ignoreSmallerThan = 0.4f;
        [SerializeField] private bool ignoreDynamicRigidbodies = true;

        [Header("Distances")]
        [SerializeField] private float pivotHeight = 1.5f;
        [SerializeField] private float minPivotHeight = 0.2f;
        [SerializeField] private float firstPersonThreshold = 1.0f;

        [Header("Near Clip")]
        [SerializeField] private float closeDistance = 1.0f;
        [SerializeField] private float closeNearClip = 0.05f;

        [Header("Smoothing")]
        [SerializeField] private float returnSmoothTime = 0.3f;

        private readonly RaycastHit[] _hits = new RaycastHit[32];
        private Collider[] _playerColliders = new Collider[0];
        private Transform _cachedPlayer;

        private float _pivotUp = -1f;
        private float _shoulder = -1f;
        private float _distance = -1f;
        private float _pivotVel, _shoulderVel, _distanceVel;

        public float CurrentDistance { get; private set; }
        public float DesiredDistance { get; private set; }
        public bool IsObstructed { get; private set; }

        private Camera _camera;
        private float _defaultNearClip = -1f;

        private void Awake()
        {
            if (follow == null)
                follow = GetComponent<CameraFollow>();
            _camera = GetComponent<Camera>();
            if (_camera != null)
                _defaultNearClip = _camera.nearClipPlane;
        }

        private void OnEnable() => ResetState();

        private void OnDisable()
        {
            if (_camera != null && _defaultNearClip > 0f)
                _camera.nearClipPlane = _defaultNearClip;
        }

        private void UpdateNearClip(bool close)
        {
            if (_camera == null || _defaultNearClip <= 0f)
                return;
            float target = close ? Mathf.Min(closeNearClip, _defaultNearClip) : _defaultNearClip;
            if (!Mathf.Approximately(_camera.nearClipPlane, target))
                _camera.nearClipPlane = target;
        }

        public void ResetState()
        {
            _pivotUp = _shoulder = _distance = -1f;
            _pivotVel = _shoulderVel = _distanceVel = 0f;
        }

        private void LateUpdate()
        {
            if (follow == null || follow.player == null)
                return;

            Transform player = follow.player;
            if (player != _cachedPlayer)
            {
                _cachedPlayer = player;
                _playerColliders = player.GetComponentsInChildren<Collider>(true);
                ResetState();
            }

            Vector3 desired = transform.position;
            Vector3 anchor = player.position;
            Vector3 basePivot = anchor + Vector3.up * pivotHeight;
            Vector3 offset = desired - basePivot;

            if (offset.magnitude < firstPersonThreshold)
            {
                ResetState();
                UpdateNearClip(false);
                IsObstructed = false;
                CurrentDistance = DesiredDistance = offset.magnitude;
                return;
            }

            Vector3 right = transform.right;
            float shoulderTarget = Vector3.Dot(offset, right);
            Vector3 shoulderDir = shoulderTarget >= 0f ? right : -right;
            shoulderTarget = Mathf.Abs(shoulderTarget);
            Vector3 back = offset - right * Vector3.Dot(offset, right);
            float distanceTarget = back.magnitude;
            Vector3 backDir = distanceTarget > 1e-4f ? back / distanceTarget : -transform.forward;
            DesiredDistance = offset.magnitude;

            float pivotAllowed = pivotHeight;
            if (Cast(anchor, Vector3.up, pivotHeight, out float upHit))
                pivotAllowed = Mathf.Max(minPivotHeight, upHit - wallPadding);
            _pivotUp = Approach(_pivotUp, pivotAllowed, ref _pivotVel);
            Vector3 pivot = anchor + Vector3.up * _pivotUp;

            float shoulderAllowed = shoulderTarget;
            if (shoulderTarget > 1e-3f && Cast(pivot, shoulderDir, shoulderTarget, out float sideHit))
                shoulderAllowed = Mathf.Max(0f, sideHit - wallPadding);
            _shoulder = Approach(_shoulder, shoulderAllowed, ref _shoulderVel);
            Vector3 shoulderPoint = pivot + shoulderDir * _shoulder;

            float distanceAllowed = distanceTarget;
            if (Cast(shoulderPoint, backDir, distanceTarget, out float backHit))
                distanceAllowed = Mathf.Max(0f, backHit - wallPadding);
            _distance = Approach(_distance, distanceAllowed, ref _distanceVel);

            transform.position = shoulderPoint + backDir * _distance;

            IsObstructed = pivotAllowed < pivotHeight - 1e-3f
                           || shoulderAllowed < shoulderTarget - 1e-3f
                           || distanceAllowed < distanceTarget - 1e-3f;
            CurrentDistance = Vector3.Distance(transform.position, basePivot);
            UpdateNearClip(IsObstructed && Vector3.Distance(transform.position, pivot) < closeDistance);
        }

        private float Approach(float current, float target, ref float velocity)
        {
            if (current < 0f || target <= current)
            {
                velocity = 0f;
                return target;
            }
            return Mathf.SmoothDamp(current, target, ref velocity, returnSmoothTime);
        }

        private bool Cast(Vector3 origin, Vector3 direction, float maxDistance, out float hitDistance)
        {
            hitDistance = maxDistance;
            if (maxDistance <= 1e-4f)
                return false;

            int count = Physics.SphereCastNonAlloc(origin, castRadius, direction, _hits, maxDistance,
                collisionMask, QueryTriggerInteraction.Ignore);

            bool found = false;
            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = _hits[i];
                if (hit.distance <= 0f && hit.point == Vector3.zero)
                    continue;
                if (ShouldIgnore(hit.collider))
                    continue;
                if (hit.distance < hitDistance)
                {
                    hitDistance = hit.distance;
                    found = true;
                }
            }
            return found;
        }

        private bool ShouldIgnore(Collider col)
        {
            for (int i = 0; i < _playerColliders.Length; i++)
                if (_playerColliders[i] == col)
                    return true;

            if (ignoreDynamicRigidbodies)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                    return true;
            }

            Vector3 size = col.bounds.size;
            return Mathf.Max(size.x, Mathf.Max(size.y, size.z)) < ignoreSmallerThan;
        }

        private void OnValidate()
        {
            castRadius = Mathf.Max(0.05f, castRadius);
            returnSmoothTime = Mathf.Max(0.01f, returnSmoothTime);
        }
    }
}
