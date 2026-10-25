using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform destination;
    [SerializeField] private string destinationName;

    [Header("Arrival Effect")]
    [SerializeField] private ParticleSystem arrivalParticles;

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 1f;

    private static float nextTeleportTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextTeleportTime)
            return;

        if (!other.CompareTag("Player"))
            return;

        //if (destination == null)
        //{
        //    Debug.LogWarning("Portal destination is not assigned!");
        //    return;
        //}

        GameObject destinationObject = GameObject.Find(destinationName);

        if (destinationObject == null)
        {
            Debug.LogWarning(
                "Destination not found: " + destinationName
            );
            return;
        }

        Transform destination = destinationObject.transform;

        CharacterController controller =
            other.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        other.transform.SetPositionAndRotation(
            destination.position,
            destination.rotation
        );

        if (controller != null)
            controller.enabled = true;

        Physics.SyncTransforms();

        nextTeleportTime = Time.time + cooldownTime;

        if (arrivalParticles != null)
        {
            Vector3 effectPosition =
                other.transform.position + Vector3.up;

            ParticleSystem effect = Instantiate(
                arrivalParticles,
                effectPosition,
                Quaternion.identity
            );

            effect.Play();

            Destroy(effect.gameObject, 3f);
        }

        Debug.Log("Player teleported to " + destination.name);
    }
}