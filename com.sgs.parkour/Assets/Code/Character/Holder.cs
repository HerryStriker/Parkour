using UnityEngine;

public class Holder : MonoBehaviour
{
    Camera mainCamera;
    public Transform cameraTransform;

    void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
    }

    void Update()
    {
        CheckGrounded();
    }

    [Header("Ground Check")]
    public bool IsGrounded { get; private set; } = false;
    [SerializeField] float groundedHeight = 1f;
    [SerializeField] float groundedRadius = 0.2f;
    [SerializeField] LayerMask groundMask;

    void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.up * groundedHeight + transform.position, groundedRadius, groundMask);
    }


    private void OnDrawGizmos()
    {
        var color = IsGrounded ? Color.green : Color.yellow;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.up * groundedHeight + transform.position, groundedRadius);
    }
}
