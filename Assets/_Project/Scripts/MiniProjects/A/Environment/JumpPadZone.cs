
using UnityEngine;

public class JumpPadZone : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 4f;

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
        if (!_player.IsGrounded)
            return;

        _player.Jump(jumpHeight);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _player = null;
        Debug.Log("Exited Jump Pad");
    }
}
