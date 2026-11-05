using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters
{
    /// <summary>
    /// 第三人稱鏡頭輸入（mini project 4.A.2）：滑鼠移動繞角色旋轉鏡頭、滾輪拉近拉遠。
    /// 點擊 Game 畫面鎖定游標開始轉動，Esc 釋放游標。
    /// 放在帶有 CinemachineOrbitalFollow 的 CinemachineCamera 上。
    /// </summary>
    [RequireComponent(typeof(CinemachineOrbitalFollow))]
    public class ThirdPersonCameraInput : MonoBehaviour
    {
        [SerializeField] private float _mouseSensitivity = 0.15f;   // 度/像素
        [SerializeField] private float _zoomSensitivity = 0.001f;   // 每單位滾輪改變的半徑倍率
        [SerializeField] private bool _invertY;

        private CinemachineOrbitalFollow _orbital;

        private void Awake()
        {
            _orbital = GetComponent<CinemachineOrbitalFollow>();
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null)
                return;

            UpdateCursorLock(mouse);
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            Vector2 delta = mouse.delta.ReadValue() * _mouseSensitivity;

            // 水平：繞角色一圈可循環
            float yaw = _orbital.HorizontalAxis.Value + delta.x;
            _orbital.HorizontalAxis.Value = Mathf.Repeat(yaw + 180f, 360f) - 180f;

            // 垂直：限制在 VerticalAxis 的範圍內
            float pitch = _orbital.VerticalAxis.Value + (_invertY ? delta.y : -delta.y);
            _orbital.VerticalAxis.Value = Mathf.Clamp(pitch, _orbital.VerticalAxis.Range.x, _orbital.VerticalAxis.Range.y);

            // 滾輪：改變半徑倍率
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float radial = _orbital.RadialAxis.Value - scroll * _zoomSensitivity;
                _orbital.RadialAxis.Value = Mathf.Clamp(radial, _orbital.RadialAxis.Range.x, _orbital.RadialAxis.Range.y);
            }
        }

        private static void UpdateCursorLock(Mouse mouse)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
