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
            Debug.Log("GROUND: Enter");
        };

        holder.GroundCheck.OnEnter += (holder, args) => {
            InAir = true;
            Debug.Log("GROUND: Exit");
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

    [Header("JUMP")]
    public bool IsJumping = false;

    float time;
    float normalizedTime 
    {
        get
        {
            return time / MAX_TIME;
        }
    }
    const float MIN_TIME = 0;
    const float MAX_TIME = 3;

    const int MAX_JUMP_COUNT = 10;
    int jumpCount;

    Vector3 direction;

    [Space(10)]
    [Header("UP FORCE")]
    [SerializeField, Range(0,1)] float MIN_FORCE_ANGLE = 0;
    [SerializeField, Range(0,1)] float MAX_FORCE_ANGLE = 1;

    [Space(10)]
    [Header("UP AND FORWARD NORMALIZED FORCE")]
    [SerializeField, Range(0,1)] float forwardForce = 0.1f;
    [SerializeField, Range(0,1)] float upForce = 0.1f;

    [SerializeField] float MIN_FORCE = 0f;
    [SerializeField] float MAX_FORCE = 15f;

    void DirectionalJumpLogic()
    {
        // CAN JUMP IF IS ON GROUND
        var canJump = IsJumping && time < MAX_TIME && holder.GroundCheck.IsColliding;

        //CAN JUMP IF IS ON AIR AND HAVE DOUBLE JUMP LEFT
        var canDoubleJump = InputManager.Instance.IsJumping && IsJumping && time < MAX_TIME && !holder.GroundCheck.IsColliding && jumpCount > 0;

        time += canJump || canDoubleJump ? Time.deltaTime : -Time.deltaTime;
        time = Mathf.Clamp(time, MIN_TIME, MAX_TIME);
        
        float angle = Mathf.Lerp(MIN_FORCE_ANGLE, MAX_FORCE_ANGLE, normalizedTime);

        float cameraRotation = holder.CameraTransform.eulerAngles.y;
        Vector3 fowardDirection = Quaternion.Euler(0,cameraRotation,0) * (Vector3.forward * forwardForce);
        Vector3 upDirection = new Vector3(0,angle,0) * upForce;

        float force = Mathf.Lerp(MIN_FORCE, MAX_FORCE, normalizedTime);
        
        direction = (fowardDirection + upDirection) * force;
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

    void Move(Vector2 direction)
    {
        if (EnableMovement && holder.GroundCheck.IsColliding && !IsJumping)
        {
            var dir = holder.Velocity * direction.magnitude * transform.forward;
            var targetSpeed = holder.Velocity - rb.linearVelocity.magnitude;
            if(rb.linearVelocity.magnitude < holder.Velocity)
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

    private void OnDrawGizmos()
    {
        if(holder == null) return;

        float angle = Mathf.Lerp(MIN_FORCE_ANGLE, MAX_FORCE_ANGLE, normalizedTime);
        float force = Mathf.Lerp(MIN_FORCE, MAX_FORCE, normalizedTime);

        float cameraRotation = holder.CameraTransform.eulerAngles.y;
        Vector3 fowardDirection = Quaternion.Euler(0,cameraRotation,0) * (Vector3.forward * forwardForce);
        Vector3 upDirection = new Vector3(0,angle,0) * upForce;


        // Gizmos.DrawRay(transform.position, fowardDirection * force);

        // STATIC MAX UP ANGLE
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (fowardDirection + (new Vector3(0,MAX_FORCE_ANGLE,0) * upForce)) * force);

        // STATIC MIN UP ANGLE
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (fowardDirection + (new Vector3(0,MIN_FORCE_ANGLE,0) * upForce)) * force);

        // LERP UP ANGLE
        var color = Color.Lerp(Color.yellow, Color.blue, normalizedTime);
        Gizmos.color = normalizedTime > 0 ? color : Color.clear;
        Gizmos.DrawRay(transform.position, direction);
    }
}
 
