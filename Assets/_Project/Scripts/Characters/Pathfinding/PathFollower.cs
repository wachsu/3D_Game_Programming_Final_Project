using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters.Pathfinding
{
    // 讓角色沿著 A* 路徑走到目標
    // 連續移動、平滑轉向、走路動畫，走到一半可以隨時換目標
    [RequireComponent(typeof(CharacterController))]
    public class PathFollower : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AStarGrid _grid;
        [SerializeField] private PathMarkers _markers;   // 可以不拖，不拖就不放小球
        [SerializeField] private Animator _animator;     // 空白時自動找子物件

        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 2f;          // m/s
        [SerializeField] private float _acceleration = 6f;       // 加速度 m/s²
        [SerializeField] private float _turnSpeed = 360f;        // 每秒最多轉幾度
        [SerializeField] private float _arriveDistance = 0.1f;   // 離路徑點多近算走到了
        [SerializeField] private float _slowDownDistance = 1f;   // 離終點多遠開始減速
        [SerializeField] private float _gravity = -20f;

        private static readonly int SpeedParam = Animator.StringToHash("Speed");

        private CharacterController _controller;
        private List<Vector3> _path = new List<Vector3>();
        private int _index;               // 正在走向第幾個路徑點
        private float _speed;             // 目前的水平速度
        private float _verticalVelocity;  // 目前的垂直速度 (重力)

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        // 設定新目標：重新找路、放小球。走到一半也可以呼叫
        public void SetDestination(Vector3 destination)
        {
            GridNode goal = AStarPathfinder.FindPath(_grid, transform.position, destination);
            List<Vector3> newPath = AStarPathfinder.BuildPath(goal);

            // 找不到路：保留原本的路線，繼續走
            if (newPath.Count == 0)
            {
                Debug.Log("[PathFollower] 找不到路，忽略這次點擊");
                return;
            }

            _path = newPath;
            _index = 1;   // 第 0 個點是腳下那格的中心，跳過它
            if (_markers != null)
                _markers.Show(_path);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // ① 已經走到的路徑點就跳過，換下一個 (一幀可能跳過好幾個)
            while (_index < _path.Count && FlatDistance(transform.position, _path[_index]) < _arriveDistance)
                _index++;

            // ② 走完了：清掉路徑和小球
            if (_index >= _path.Count && _path.Count > 0)
            {
                _path.Clear();
                if (_markers != null)
                    _markers.Clear();
            }

            // ③ 算出要往哪走、要走多快
            Vector3 direction = Vector3.zero;
            float targetSpeed = 0f;
            if (_index < _path.Count)
            {
                Vector3 toWaypoint = _path[_index] - transform.position;
                toWaypoint.y = 0f;
                direction = toWaypoint.normalized;

                // 快到終點時慢慢減速，不要急煞
                float distanceToEnd = FlatDistance(transform.position, _path[_path.Count - 1]);
                targetSpeed = _walkSpeed * Mathf.Clamp01(distanceToEnd / _slowDownDistance);
            }
            _speed = Mathf.MoveTowards(_speed, targetSpeed, _acceleration * dt);

            // ④ 平滑轉向：每秒最多轉 _turnSpeed 度，不會一下子轉過去
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * dt);
            }

            // ⑤ 重力 (和 PlayerMotor 一樣)，讓角色貼著地板
            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            _verticalVelocity += _gravity * dt;

            // ⑥ 移動：每幀只走一小步，所以不會瞬移
            Vector3 velocity = direction * _speed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * dt);

            // ⑦ 走路動畫：把速度交給 Animator
            if (_animator != null)
                _animator.SetFloat(SpeedParam, _speed, 0.1f, dt);
        }

        // 只看水平距離 (忽略高度)
        private static float FlatDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }
    }
}