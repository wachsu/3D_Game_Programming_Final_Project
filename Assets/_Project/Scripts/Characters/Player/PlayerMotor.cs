using Game.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters
{
    /// <summary>
    /// 玩家移動（mini project 4.A.2）：WASD 依攝影機方向移動、Shift 跑步、R 跳躍，
    /// 並把實際水平速度傳給 Animator 的 Speed 參數切換 Idle / Walk / Run。
    /// 實作 IPlayerMotor，供傳送門、加速地板等元件呼叫。
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour, IPlayerMotor
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _acceleration = 15f;   // m/s²，速度變化的快慢
        [SerializeField] private float _turnSpeed = 720f;     // 度/秒

        [Header("Jump & Gravity")]
        [SerializeField] private float _jumpHeight = 1f;
        [SerializeField] private float _gravity = -20f;

        [Header("References")]
        [SerializeField] private Transform _cameraTransform;  // 空白時使用 Main Camera
        [SerializeField] private Animator _animator;          // 空白時自動找子物件

        [Header("Debug")]
        [SerializeField] private bool _showDebug;             // 勾選後在畫面左上角顯示輸入與速度

        private static readonly int SpeedParam = Animator.StringToHash("Speed");

        private CharacterController _controller;
        private InputAction _moveAction, _runAction, _jumpAction;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private float _speedMultiplier = 1f;
        private float _multiplierTimeLeft;
        private Vector2 _lastInput;

        public bool IsMoving => _horizontalVelocity.sqrMagnitude > 0.01f;
        public Vector3 Velocity => _horizontalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            _moveAction = new InputAction("Move", InputActionType.Value);
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _runAction = new InputAction("Run", InputActionType.Button, "<Keyboard>/leftShift");
            _jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/r");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _runAction.Enable();
            _jumpAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _runAction.Disable();
            _jumpAction.Disable();
        }

        private void OnDestroy()
        {
            _moveAction.Dispose();
            _runAction.Dispose();
            _jumpAction.Dispose();
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            UpdateSpeedMultiplier(dt);

            // 依攝影機方向換算移動方向（只取水平分量）
            Vector2 input = Vector2.ClampMagnitude(_moveAction.ReadValue<Vector2>(), 1f);
            _lastInput = input;
            Vector3 forward = _cameraTransform != null ? _cameraTransform.forward : Vector3.forward;
            Vector3 right = _cameraTransform != null ? _cameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            Vector3 moveDir = forward.normalized * input.y + right.normalized * input.x;

            float targetSpeed = (_runAction.IsPressed() ? _runSpeed : _walkSpeed) * _speedMultiplier;
            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, moveDir * targetSpeed, _acceleration * dt);

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * dt);
            }

            // 重力與跳躍
            bool grounded = _controller.isGrounded;
            if (grounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;   // 保持貼地
            if (grounded && _jumpAction.WasPressedThisFrame())
                Jump(_jumpHeight);
            _verticalVelocity += _gravity * dt;

            CollisionFlags flags = _controller.Move((_horizontalVelocity + Vector3.up * _verticalVelocity) * dt);

            // 側面撞到障礙物時以實際速度為準，避免原地播走路動畫
            if ((flags & CollisionFlags.Sides) != 0)
            {
                Vector3 actual = _controller.velocity;
                actual.y = 0f;
                _horizontalVelocity = actual;
            }

            if (_animator != null)
                _animator.SetFloat(SpeedParam, _horizontalVelocity.magnitude, 0.1f, dt);
        }

        private void OnGUI()
        {
            if (!_showDebug)
                return;

            string text =
                $"Input: {_lastInput}\n" +
                $"Keyboard W: {(Keyboard.current != null && Keyboard.current.wKey.isPressed)}\n" +
                $"Grounded: {_controller.isGrounded}\n" +
                $"Speed: {_horizontalVelocity.magnitude:F2} m/s (x{_speedMultiplier})\n" +
                $"Position: {transform.position}\n" +
                $"Animator: {(_animator != null ? _animator.name : "none")}  Camera: {(_cameraTransform != null ? _cameraTransform.name : "none")}";
            GUI.Box(new Rect(10, 10, 360, 110), GUIContent.none);
            GUI.Label(new Rect(18, 14, 350, 105), text);
        }

        private void UpdateSpeedMultiplier(float dt)
        {
            if (_multiplierTimeLeft <= 0f)
                return;
            _multiplierTimeLeft -= dt;
            if (_multiplierTimeLeft <= 0f)
                _speedMultiplier = 1f;
        }

        // ---- IPlayerMotor ----

        /// <summary>改變移動速度倍率；duration ≤ 0 表示持續到下次呼叫為止。</summary>
        public void SetSpeedMultiplier(float multiplier, float duration)
        {
            _speedMultiplier = Mathf.Max(0f, multiplier);
            _multiplierTimeLeft = duration > 0f ? duration : float.PositiveInfinity;
        }

        /// <summary>以指定高度起跳（外部呼叫時不檢查是否著地，例如彈跳床）。</summary>
        public void Jump(float height)
        {
            _verticalVelocity = Mathf.Sqrt(2f * Mathf.Max(0f, height) * -_gravity);
        }

        /// <summary>依規格：關 CharacterController → 設位置 → 開 → SyncTransforms。</summary>
        public void Teleport(Vector3 pos, Quaternion rot)
        {
            _controller.enabled = false;
            transform.SetPositionAndRotation(pos, rot);
            _controller.enabled = true;
            Physics.SyncTransforms();
            _horizontalVelocity = Vector3.zero;
            _verticalVelocity = 0f;
        }
    }
}
