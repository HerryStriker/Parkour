using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Locomotion : MonoBehaviour
{
    Holder holder;
    Rigidbody rb;

    public bool EnableMovement {get; private set; } = true;
    public bool CancelAction {get; private set; } = false;

    public void EnableCharacterMovement(bool enable)
    {
        EnableMovement = enable;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = enable;
    }

    private void Awake() 
    {
        holder = GetComponent<Holder>();
        rb = GetComponent<Rigidbody>();

        JumpCount = MAX_JUMP_COUNT;
    }

    private void Start() {
        holder.GroundCheck.OnEnter += (holderObj, args) => {
            holder.InAir = false;

            if(IsJumping)
            {
                IsJumping = false;
                holder.ResetTime();
            }
            Debug.Log("GROUND: Enter");
        };

        holder.GroundCheck.OnEnter += (holderObj, args) => {
            holder.InAir = true;
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
        }
    }

    [Header("JUMP")]
    public bool IsJumping = false;

    public event EventHandler OnDoubleJumpChangedCallback;

    [Header("DOUBLE JUMP")]
    [field: SerializeField] public int JumpCount {get; private set;}
    const int MAX_JUMP_COUNT = 10;

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
        float angle = Mathf.Lerp(MIN_FORCE_ANGLE, MAX_FORCE_ANGLE, holder.NormalizedTime);

        float cameraRotation = holder.CameraTransform.eulerAngles.y;
        Vector3 fowardDirection = Quaternion.Euler(0,cameraRotation,0) * (Vector3.forward * forwardForce);
        Vector3 upDirection = new Vector3(0,angle,0) * upForce;

        float force = Mathf.Lerp(MIN_FORCE, MAX_FORCE, holder.NormalizedTime);
        
        direction = (fowardDirection + upDirection) * force;
    }

    void Bouncing(object holderObj, EventArgs args) 
    {
        if(holder.InAir)
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
            holder.ResetTime();
            return;
        }
        var isGrouded = holder.GroundCheck.IsColliding;

        if(isGrouded || JumpCount > 0) 
        {
            rb.AddForce(direction, ForceMode.Impulse);
            holder.ResetTime();
            JumpCount--;
            OnDoubleJumpChangedCallback?.Invoke(this, new EventArgs());
        }
    }

    [SerializeField, Range(-1f, 0f)] float fallingThreshold = -.1f;
    public bool IsFalling {
        get {
            return rb.linearVelocity.y < fallingThreshold;
        }
    }

    void Move(Vector2 direction)
    {
        if (EnableMovement && holder.GroundCheck.IsColliding && !IsJumping)
        {
            var dir = holder.Velocity * direction.magnitude * transform.forward;
            

            var movedir = new Vector3(dir.x , rb.linearVelocity.y , dir.z);
            rb.linearVelocity = direction.magnitude > 0 ? movedir : Vector3.zero; 

        }
    }

#region Utils
    public void MovePosition(Vector3 position)
    {
        rb.linearVelocity = Vector3.zero;
        rb.MovePosition(position);
    }  
#endregion

    private void OnDrawGizmos()
    {
        if(holder == null) return;

        float angle = Mathf.Lerp(MIN_FORCE_ANGLE, MAX_FORCE_ANGLE, holder.NormalizedTime);
        float force = Mathf.Lerp(MIN_FORCE, MAX_FORCE, holder.NormalizedTime);

        float cameraRotation = holder.CameraTransform.eulerAngles.y;
        Vector3 fowardDirection = Quaternion.Euler(0,cameraRotation,0) * (Vector3.forward * forwardForce);


        // Gizmos.DrawRay(transform.position, fowardDirection * force);

        // STATIC MAX UP ANGLE
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (fowardDirection + (new Vector3(0,MAX_FORCE_ANGLE,0) * upForce)) * force);

        // STATIC MIN UP ANGLE
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (fowardDirection + (new Vector3(0,MIN_FORCE_ANGLE,0) * upForce)) * force);

        // LERP UP ANGLE
        var color = Color.Lerp(Color.yellow, Color.blue, holder.NormalizedTime);
        Gizmos.color = holder.NormalizedTime > 0 ? color : Color.clear;
        Gizmos.DrawRay(transform.position, direction);
    }
}
 
