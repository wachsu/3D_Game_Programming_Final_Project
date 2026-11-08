
using UnityEngine;

public class BallPitZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.4f;
    [SerializeField] private float ballPitStepOffset = 0.05f;

    private CharacterController playerController;
    private float originalStepOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EnvironmentPlayerMovement movement = other.GetComponent<EnvironmentPlayerMovement>();

        if (movement != null)
        {
            movement.SetSpeedMultiplier(speedMultiplier);
        }

        playerController = other.GetComponent<CharacterController>();

        if (playerController != null)
        {
            originalStepOffset = playerController.stepOffset;
            playerController.stepOffset = ballPitStepOffset;
        }

        Debug.Log("Entered Ball Pit");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EnvironmentPlayerMovement movement = other.GetComponent<EnvironmentPlayerMovement>();

        if (movement != null)
        {
            movement.SetSpeedMultiplier(1f);
        }

        if (playerController != null)
        {
            playerController.stepOffset = originalStepOffset;
            playerController = null;
        }

        Debug.Log("Exited Ball Pit");
    }
}
