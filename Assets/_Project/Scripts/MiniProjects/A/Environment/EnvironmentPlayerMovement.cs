
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class EnvironmentPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform;

    private float speedMultiplier = 1f;

    [Header("Character Rotation")]
    [SerializeField] private Transform characterModel;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("Ball Pit Push")]
    [SerializeField] private LayerMask ballLayer;
    [SerializeField] private float ballPushSpeed = 2f;

    private CharacterController _controller;
    private float verticalVelocity;

    public bool IsGrounded => _controller != null && _controller.isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (_controller == null)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;
        }

        Vector3 direction = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            direction = forward * input.y + right * input.x;
            direction = Vector3.ClampMagnitude(direction, 1f);
        }

        if (characterModel != null && direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            characterModel.rotation = Quaternion.Slerp(
                characterModel.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (_controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Jump(jumpHeight);
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 movement = direction * moveSpeed * speedMultiplier + Vector3.up * verticalVelocity;
        _controller.Move(movement * Time.deltaTime);

        UpdateAnimation(direction);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void Jump(float height)
    {
        if (_controller == null || !_controller.isGrounded)
            return;
        verticalVelocity = Mathf.Sqrt(height * -2f * gravity);
    }

    private void UpdateAnimation(Vector3 direction)
    {
        if (animator == null)
            return;

        bool isMoving = direction.sqrMagnitude > 0.001f;
        bool isRunning = isMoving && speedMultiplier > 1.01f;
        bool isGrounded = _controller.isGrounded;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);

        if (isMoving && !isRunning && speedMultiplier < 0.99f)
        {
            animator.speed = 0.5f;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic)
            return;

        if ((ballLayer.value & (1 << hit.gameObject.layer)) == 0)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDirection = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        ).normalized;

        if (pushDirection.sqrMagnitude < 0.001f)
            return;

        rb.linearVelocity = new Vector3(
            pushDirection.x * ballPushSpeed,
            rb.linearVelocity.y,
            pushDirection.z * ballPushSpeed
        );
    }
}
