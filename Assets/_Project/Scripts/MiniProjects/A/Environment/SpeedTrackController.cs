
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrackController : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1.8f;

    private EnvironmentPlayerMovement playerMovement;
    private readonly HashSet<SpeedTrackZone> occupiedZones = new HashSet<SpeedTrackZone>();

    public void EnterZone(SpeedTrackZone zone, Collider player)
    {
        if (!player.CompareTag("Player"))
            return;

        EnvironmentPlayerMovement movement = player.GetComponent<EnvironmentPlayerMovement>();

        if (movement == null)
            return;

        playerMovement = movement;
        occupiedZones.Add(zone);
        playerMovement.SetSpeedMultiplier(speedMultiplier);
        Debug.Log("Speed Track active. Zones: " + occupiedZones.Count);
    }

    public void ExitZone(SpeedTrackZone zone, Collider player)
    {
        if (!player.CompareTag("Player"))
            return;

        occupiedZones.Remove(zone);

        if (occupiedZones.Count == 0 && playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(1f);
            playerMovement = null;
            Debug.Log("Left Speed Track. Speed restored.");
        }
    }
}
