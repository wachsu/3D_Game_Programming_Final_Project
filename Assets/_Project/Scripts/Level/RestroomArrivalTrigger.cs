using System;
using UnityEngine;

namespace Game.Level
{
    [RequireComponent(typeof(Collider))]
    public class RestroomArrivalTrigger : MonoBehaviour
    {
        public static event Action<RestroomArrivalTrigger> PlayerArrived;

        [SerializeField] private float cooldown = 3f;

        private float _nextAllowedTime;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            if (Time.time < _nextAllowedTime)
                return;

            _nextAllowedTime = Time.time + cooldown;
            PlayerArrived?.Invoke(this);
        }
    }
}
