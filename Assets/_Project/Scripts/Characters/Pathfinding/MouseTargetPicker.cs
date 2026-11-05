using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters.Pathfinding
{
    // 滑鼠左鍵點地板，設定目標點 (mini project 4.B.1)
    public class MouseTargetPicker : MonoBehaviour
    {
        [SerializeField] private Camera _camera;              // 沒拖的話自動用 Main Camera
        [SerializeField] private LayerMask _floorMask;        // 只偵測地板
        [SerializeField] private Transform _target;           // 點到的位置會把它搬過去
        [SerializeField] private PathFollower _follower;   // 點到地板時，叫角色走過去
        [SerializeField] private float _maxDistance = 200f;   // 射線最遠多長

        private void Awake()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || _target == null)
                return;

            // 這一幀沒有「剛按下」左鍵，就不做事
            if (!mouse.leftButton.wasPressedThisFrame)
                return;

            // 從鏡頭穿過滑鼠位置，射出一條射線
            Vector2 screenPosition = mouse.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(screenPosition);

            // 射線打到地板，就把目標搬到打到的點
            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _floorMask))
            {
                _target.position = hit.point;
                Debug.Log($"[MouseTargetPicker] 新目標 {hit.point}");
                if (_follower != null)
                    _follower.SetDestination(hit.point);
            }
        }
    }
}