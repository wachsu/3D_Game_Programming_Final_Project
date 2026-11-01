
using UnityEngine;

public class JumpPadZone : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float bounceDelay = 0.3f;

    private float nextBounceTime = 0f;

    private EnvironmentPlayerMovement _player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = other.GetComponent<EnvironmentPlayerMovement>();

        if (_player != null)
        {
            Debug.Log("Entered Jump Pad");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_player == null)
            return;

        if (Time.time < nextBounceTime)
            return;

        if (!_player.IsGrounded)
            return;

        _player.Jump(jumpHeight);

        nextBounceTime = Time.time + bounceDelay;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = null;

        Debug.Log("Exited Jump Pad");
    }
}
