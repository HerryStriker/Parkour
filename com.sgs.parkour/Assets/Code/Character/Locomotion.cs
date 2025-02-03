using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Locomotion : MonoBehaviour
{
    Holder holder;
    Rigidbody rb;
    CapsuleCollider capsuleCollider;

    public bool EnableMovement {get; private set; } = true;
    public bool CancelAction {get; private set; } = false;

    public void EnableCharacterMovement(bool enable)
    {
        EnableMovement = enable;
    }

    private void Awake() 
    {

        holder = GetComponent<Holder>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.center += new Vector3(0, 1, 0);
        capsuleCollider.height = 2;
    }

    private void Start() {
        //InputManager.Instance.OnJumpPerformed += OnJumpPerformed;
        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;
        InputManager.Instance.OnCancel += OnCancel;

        InputManager.Instance.EnableMove();
        InputManager.Instance.EnableJump();
        InputManager.Instance.EnableCancelAction();
    }
    private void Update() 
    {
        var moveDirection = InputManager.Instance.Move;

        DirectionalJumpLogic();
        Move(moveDirection);
    }

    void OnCancel(object inputManager, EventArgs args)
    {
        // CANCELATION ACTION IS CANCELLING NEXT ACTION
        // FIX THIS NEXT TIME

        if (!CancelAction)
        {
            CancelAction = true;
        }
    }

    [Header("Super Jump")]    
    const float MIN_TIME = 0;
    const float MAX_TIME = 3;

    float time;
    Vector3 jumpDirection;

    const float MIN_JUMP_ANGLE = 0f;
    const float MAX_JUMP_ANGLE = 5f;

    
    [SerializeField] float directionForce = 1f;
    [SerializeField] float forwardForce = 5f;
    [SerializeField] float upForce = 5;

    void DirectionalJumpLogic()
    {
        time += InputManager.Instance.IsJumping && time < MAX_TIME && holder.IsGrounded ? Time.deltaTime : -Time.deltaTime;
        time = Mathf.Clamp(time, MIN_TIME, MAX_TIME);
        
        float normalized = time / MAX_TIME;

        float angle = Mathf.Lerp(MIN_JUMP_ANGLE, MAX_JUMP_ANGLE, normalized);

        float cameraRotation = holder.cameraTransform.eulerAngles.y;
        Vector3 cameraForward = Quaternion.Euler(0,cameraRotation, 0) * Vector3.forward;

        jumpDirection = forwardForce * cameraForward + new Vector3(0,angle,0) * upForce;
        Debug.DrawRay(transform.position, jumpDirection);
    }

    void OnJumpCanceled(object inputManager, EventArgs args) {
        if(CancelAction)
        {
            CancelAction = false;
            time = 0;
            return;
        }

        if(holder.IsGrounded) 
        {
            rb.AddForce(jumpDirection * directionForce, ForceMode.Impulse);
            time = 0;
        }
    }

    const float JUMP_FORCE = 60f;
    void OnJumpPerformed(object inputManager, EventArgs args) {
        if(!holder.IsGrounded) return;

        rb.AddForce(Vector3.up * JUMP_FORCE ,ForceMode.Impulse);
    }

    const float CHRACTER_SPEED = 6f;
    void Move(Vector2 direction)
    {
        if (EnableMovement && holder.IsGrounded && !InputManager.Instance.IsJumping)
        {
            var dir = CHRACTER_SPEED * direction.magnitude * transform.forward;
            rb.linearVelocity = new Vector3(dir.x , rb.linearVelocity.y , dir.z);
        }
    }

    
}
 
