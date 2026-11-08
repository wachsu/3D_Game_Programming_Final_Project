
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CleaningCartPositionController : MonoBehaviour
{
    [Header("Cart Positions")]
    [SerializeField] private Transform[] positions;

    [Header("Testing")]
    [SerializeField] private bool enableKeyboardTest = true;

    private Rigidbody _rb;
    private int _currentIndex = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        MoveToPosition(0);
    }

    private void Update()
    {
        if (!enableKeyboardTest || Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            MoveToNextPosition();
        }
    }

    public void MoveToNextPosition()
    {
        if (positions == null || positions.Length == 0)
            return;

        int nextIndex = (_currentIndex + 1) % positions.Length;

        MoveToPosition(nextIndex);
    }

    private void MoveToPosition(int index)
    {
        if (positions == null || index < 0 || index >= positions.Length)
            return;

        Transform target = positions[index];

        if (target == null)
            return;

        _currentIndex = index;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _rb.position = target.position;
        _rb.rotation = target.rotation;

        _rb.Sleep();

        Physics.SyncTransforms();

        Debug.Log("Cleaning cart moved to position " + index);
    }
}
