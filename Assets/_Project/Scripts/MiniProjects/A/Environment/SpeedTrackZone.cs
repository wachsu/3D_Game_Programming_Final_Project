
using UnityEngine;

public class SpeedTrackZone : MonoBehaviour
{
    private SpeedTrackController controller;

    private void Awake()
    {
        controller = GetComponentInParent<SpeedTrackController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controller != null)
            controller.EnterZone(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (controller != null)
            controller.ExitZone(this, other);
    }
}
