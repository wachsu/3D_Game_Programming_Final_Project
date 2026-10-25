
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public GameObject characterModel;

    public float distance = 8f;
    public float minDistance = 3f;
    public float maxDistance = 12f;
    public float zoomSpeed = 0.1f;

    public float shoulderOffset = 1.5f;

    private bool isFirstPerson = false;

    public float mouseSensitivity = 0.15f;

    private float yaw = 0f;
    private float pitch = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }
    }

    void LateUpdate()
    {
        if (player == null || Mouse.current == null)
            return;

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isFirstPerson = !isFirstPerson;

            if (characterModel != null)
            {
                characterModel.SetActive(!isFirstPerson);
            }
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -10f, 25f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (isFirstPerson)
        {
            Vector3 eyePosition =
                player.position + Vector3.up * 0.6f;

            transform.position = eyePosition;
            transform.rotation = rotation;
        }
        else
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(
                distance,
                minDistance,
                maxDistance
            );

            Vector3 targetPosition =
                player.position + Vector3.up * 1.5f;

            Vector3 offset = rotation * new Vector3(
                shoulderOffset,
                0f,
                -distance
            );

            transform.position = targetPosition + offset;

            transform.rotation = rotation;
        }
    }
}
