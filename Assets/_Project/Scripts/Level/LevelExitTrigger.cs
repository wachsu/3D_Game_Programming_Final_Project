using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Level
{
    public class LevelExitTrigger : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private int currentFloor = -1;
        [SerializeField] private int nextFloor = 0;
        [SerializeField] private bool isUnlocked = false;

        [Header("Teleport")]
        [SerializeField] private string destinationName;

        [Header("Effects")]
        [SerializeField] private ParticleSystem teleportEffect;
        [SerializeField] private ParticleSystem arrivalEffect;

        [Header("Cooldown")]
        [SerializeField] private float cooldownTime = 1f;

        private Transform _player;
        private static float _nextTeleportTime;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _player = other.transform;

            if (isUnlocked)
                Debug.Log("Press E to enter the next level.");
            else
                Debug.Log("Elevator locked. Complete the level first.");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.transform == _player)
            {
                _player = null;
            }
        }

        private void Update()
        {
            if (_player == null || Keyboard.current == null)
                return;

            if (!Keyboard.current.eKey.wasPressedThisFrame)
                return;

            if (!isUnlocked)
            {
                Debug.Log("You must complete this level first!");
                return;
            }

            if (Time.time < _nextTeleportTime)
                return;

            TeleportPlayer();
        }

        private void TeleportPlayer()
        {
            GameObject destinationObject =
                GameObject.Find(destinationName);

            if (destinationObject == null)
            {
                Debug.LogWarning(
                    "Destination not found: " + destinationName
                );
                return;
            }

            PlayEffect(teleportEffect, _player.position + Vector3.up);

            CharacterController controller =
                _player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            _player.SetPositionAndRotation(
                destinationObject.transform.position,
                destinationObject.transform.rotation
            );

            if (controller != null)
                controller.enabled = true;

            Physics.SyncTransforms();

            PlayEffect(arrivalEffect, _player.position + Vector3.up);

            _nextTeleportTime = Time.time + cooldownTime;

            Debug.Log(
                $"Floor changed: {currentFloor} -> {nextFloor}"
            );

            _player = null;
        }

        private void PlayEffect(
            ParticleSystem prefab,
            Vector3 position)
        {
            if (prefab == null)
                return;

            ParticleSystem effect = Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

            effect.Play();

            Destroy(effect.gameObject, 3f);
        }
    }
}