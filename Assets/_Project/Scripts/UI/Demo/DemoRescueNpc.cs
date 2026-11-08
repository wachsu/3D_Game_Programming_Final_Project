using Game.Core;
using UnityEngine;

namespace Game.UI.Demo
{
    /// <summary>
    /// 只給 4.E.3 UI 示範場景用的假 NPC（正式 NPC 由 Characters 實作 IRescueTarget）：
    /// 等待時原地小幅晃動、Wandering 時亂走、Following 時跟著玩家。
    /// </summary>
    public class DemoRescueNpc : MonoBehaviour, IRescueTarget
    {
        [SerializeField] private string _id = "npc";
        [SerializeField] private string _displayName = "NPC";
        [SerializeField] private NpcType _type = NpcType.Adult;
        [SerializeField] private float _speed = 2.5f;

        public string Id => _id;
        public string DisplayName => _displayName;
        public NpcType Type => _type;
        public RescueState State { get; set; } = RescueState.Waiting;
        public Transform Transform => transform;

        private Vector3 _home;
        private Vector3 _target;
        private float _nextPick;

        private void Start()
        {
            _home = transform.position;
            _target = _home;
        }

        private void Update()
        {
            switch (State)
            {
                case RescueState.Following:
                    var player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null) MoveToward(player.transform.position - player.transform.forward * 2f);
                    break;
                case RescueState.Wandering:
                    Wander(_home, 14f);
                    break;
                case RescueState.Waiting:
                    Wander(_home, 1.5f);
                    break;
            }
        }

        private void Wander(Vector3 center, float radius)
        {
            if (Time.time >= _nextPick || (transform.position - _target).sqrMagnitude < 0.2f)
            {
                _nextPick = Time.time + Random.Range(1.5f, 3.5f);
                Vector2 r = Random.insideUnitCircle * radius;
                _target = new Vector3(center.x + r.x, transform.position.y, center.z + r.y);
            }
            MoveToward(_target);
        }

        private void MoveToward(Vector3 point)
        {
            point.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, point, _speed * Time.deltaTime);
        }
    }
}
