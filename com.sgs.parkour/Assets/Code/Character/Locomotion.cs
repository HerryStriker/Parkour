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
    public bool InAir {get; private set; } = false;

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

        jumpCount = MAX_JUMP_COUNT;
    }

    private void Start() {
        holder.GroundCheck.OnEnter += (holder, args) => {
            InAir = false;

            if(IsJumping)
            {
                IsJumping = false;
                time = 0;
            }
            Debug.Log("Enter");
        };

        holder.GroundCheck.OnEnter += (holder, args) => {
            InAir = true;
            Debug.Log("Exit");
        };

        holder.FrontCheck.OnEnter += Bouncing;        

        //InputManager.Instance.OnJumpPerformed += OnJumpPerformed;
        holder.GroundCheck.OnEnter += OnGroundEnter;
        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;
        InputManager.Instance.OnJumpStart += OnJumpStart;
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

    void OnGroundEnter(object holder, EventArgs args)
    {
        
    }

    void OnCancel(object inputManager, EventArgs args)
    {
        // CANCELATION ACTION IS CANCELLING NEXT ACTION
        // FIX THIS NEXT TIME

        if (!CancelAction)
        {
            CancelAction = true;
            IsJumping = false;
            time = 0;
        }
    }

    [Header("Jump")]
    public bool IsJumping = false;

    float time;
    const float MIN_TIME = 0;
    const float MAX_TIME = 3;

    const int MAX_JUMP_COUNT = 10;
    int jumpCount;

    Vector3 direction;
    [SerializeField, Range(0,1)] float MIN_FORCE_ANGLE = 0;
    [SerializeField, Range(0,1)] float MAX_FORCE_ANGLE = 1;


    [SerializeField, Range(0,1)] float forwardForce = 0.1f;
    [SerializeField, Range(0,1)] float upForce = 0.1f;

    [SerializeField] float MIN_FORCE = 0f;
    [SerializeField] float MAX_FORCE = 15f;

    void DirectionalJumpLogic()
    {
        var canJump = IsJumping && time < MAX_TIME && holder.GroundCheck.IsColliding;
        var canDoubleJump = IsJumping && time < MAX_TIME && !holder.GroundCheck.IsColliding && jumpCount > 0;
        time += canJump || canDoubleJump ? Time.deltaTime : -Time.deltaTime;
        time = Mathf.Clamp(time, MIN_TIME, MAX_TIME);
        
        float normalized = time / MAX_TIME;

        float angle = Mathf.Lerp(MIN_FORCE_ANGLE, MAX_FORCE_ANGLE, normalized);

        float cameraRotation = holder.CameraTransform.eulerAngles.y;
        Vector3 fowardDirection = Quaternion.Euler(0,cameraRotation,0) * (Vector3.forward * forwardForce);
        Vector3 upDirection = new Vector3(0,angle,0) * upForce;

        float force = Mathf.Lerp(MIN_FORCE, MAX_FORCE, normalized);
        
        direction = (fowardDirection + upDirection) * force;

        Debug.DrawRay(transform.position, direction);
    }

    void Bouncing(object holder, EventArgs args) 
    {
        if(InAir)
        {
            Vector3 inversedDirection = transform.InverseTransformDirection(rb.linearVelocity);
            rb.linearVelocity = inversedDirection;
        }
    }

    void OnJumpStart(object inputManager, EventArgs args)
    {
        //Debug.Log("OnJumpStart");
        if(holder.GroundCheck.IsColliding)
        {
            IsJumping = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    void OnJumpCanceled(object inputManager, EventArgs args) {
        if(CancelAction)
        {
            CancelAction = false;
            IsJumping = false;
            time = 0;
            return;
        }
        var isGrouded = holder.GroundCheck.IsColliding;

        if(isGrouded || (!isGrouded && jumpCount > 0)) 
        {
            rb.AddForce(direction, ForceMode.Impulse);
            time = 0;
            jumpCount--;
        }
    }

    // const float JUMP_FORCE = 60f;
    // void OnJumpPerformed(object inputManager, EventArgs args) {
    //     if(!holder.IsGrounded) return;

    //     rb.AddForce(Vector3.up * JUMP_FORCE ,ForceMode.Impulse);
    // }

    const float CHRACTER_SPEED = 6f;
    void Move(Vector2 direction)
    {
        if (EnableMovement && holder.GroundCheck.IsColliding && !IsJumping)
        {
            var dir = CHRACTER_SPEED * direction.magnitude * transform.forward;
            var targetSpeed = CHRACTER_SPEED - rb.linearVelocity.magnitude;
            if(rb.linearVelocity.magnitude < CHRACTER_SPEED)
            {
                if(direction.magnitude > 0)
                {
                    rb.AddForce(new Vector3(dir.x , rb.linearVelocity.y , dir.z) * targetSpeed);
                }
                else
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }

    
}
 
