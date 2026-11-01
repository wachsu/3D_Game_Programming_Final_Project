
using UnityEngine;

public class BallPitZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EnvironmentPlayerMovement movement =
            other.GetComponent<EnvironmentPlayerMovement>();

        if (movement != null)
        {
            movement.SetSpeedMultiplier(speedMultiplier);
            Debug.Log("Entered Ball Pit - Slowed down");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EnvironmentPlayerMovement movement =
            other.GetComponent<EnvironmentPlayerMovement>();

        if (movement != null)
        {
            movement.SetSpeedMultiplier(1f);
            Debug.Log("Exited Ball Pit - Speed restored");
        }
    }
}
