using UnityEngine;

namespace Game.Characters
{
    /// <summary>
    /// 站著播 Idle 的 NPC（mini project 4.A.2）：玩家走近時平滑轉身面向玩家，離開後轉回原本的方向。
    /// </summary>
    public class NpcIdle : MonoBehaviour
    {
        [SerializeField] private float _lookRadius = 3f;
        [SerializeField] private float _turnSpeed = 180f;   // 度/秒

        private Transform _player;
        private Quaternion _restRotation;

        private void Start()
        {
            _restRotation = transform.rotation;
            var playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
                _player = playerObject.transform;
        }

        private void Update()
        {
            Quaternion target = _restRotation;

            if (_player != null)
            {
                Vector3 toPlayer = _player.position - transform.position;
                toPlayer.y = 0f;
                if (toPlayer.sqrMagnitude < _lookRadius * _lookRadius && toPlayer.sqrMagnitude > 0.01f)
                    target = Quaternion.LookRotation(toPlayer);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _turnSpeed * Time.deltaTime);
        }
    }
}
