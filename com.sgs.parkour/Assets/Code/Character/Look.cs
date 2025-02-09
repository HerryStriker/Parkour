using System;
using Unity.Mathematics;
using UnityEngine;

public class Look : MonoBehaviour
{
    Holder holder;
    Rigidbody rb;

    private void Awake() {
        holder = GetComponent<Holder>();
        rb = GetComponent<Rigidbody>();
    }


    void Start() {
        holder.FrontCheck.OnEnter += OnFrontEnter;

        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;

        InputManager.Instance.EnableLook();
    }

    void Update() {
        var mouseInput = InputManager.Instance.Look;
        var keyboardInput = InputManager.Instance.Move.normalized;

        CameraLook(mouseInput);
        Rotation(keyboardInput);
    }

    void OnFrontEnter(object holderObj, EventArgs args) 
    {
        if(!holder.GroundCheck.IsColliding)
        {
            RotateTowardsMoveDirection();
        }
    }

    void OnJumpCanceled(object inputManager, EventArgs args) 
    {
        if(holder.GroundCheck.IsColliding)
        {
            RotateTowardsCameraDirection();
        }
    }

#region Rotation
    [Header("Character Rotation")]
    [SerializeField] bool m_CharacterRotation = true;
    [SerializeField, Range(1,10)] float m_RotationVelocity = 7f;
    const float THRESHOLD = 0.1f;

    void Rotation(Vector2 input)
    {
        var cameraRotation = holder.CameraTransform.eulerAngles.y;
        var dir = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraRotation;
        var moveRotation = Quaternion.Euler(0,dir,0);
        var lerp = Quaternion.Lerp(rb.rotation, moveRotation, Time.deltaTime * m_RotationVelocity);  

        if(input.magnitude > THRESHOLD && holder.GroundCheck.IsColliding && !InputManager.Instance.IsJumping && m_CharacterRotation)
        {
            rb.MoveRotation(lerp);
        }
    }
#endregion

#region Look
    [Header("Camera Look")]
    [SerializeField] bool m_cameraRotation = true;
    [SerializeField] Vector3 m_CameraDirection = Vector3.zero;
    [SerializeField] Vector2 m_CameraInput;
    [SerializeField] Vector2 m_CameraRotation = Vector2.zero;
    [SerializeField, Range(0, 10)] float m_CameraPointOffset = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityX = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityY = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityNormalized = 2f;
    [SerializeField, Range(-180, 180)] float m_MaxRotation = 60;
    [SerializeField, Range(-180, 180)] float m_MinRotation = -60;
    [SerializeField, Range(1, 10)] float m_CameraDistance = 2.5f;
    [SerializeField] InputSetting m_CameraInputSettings;

    void CameraLook(Vector2 input)
    {
        Vector2 moveDirection = new Vector2(input.x * m_CameraSensitivityX, input.y * m_CameraSensitivityY) * m_CameraSensitivityNormalized;
        m_CameraInput = m_CameraInputSettings.GetSmoothDampSettings(moveDirection, ref m_CameraInput);

        Vector3 point = transform.position + Vector3.up * m_CameraPointOffset;

        if(m_cameraRotation) 
        {
            m_CameraRotation += m_CameraInputSettings.GetSettings(m_CameraInput.y, m_CameraInput.x);
        }

        m_CameraRotation.x = Mathf.Clamp(m_CameraRotation.x, m_MinRotation, m_MaxRotation);

        if (m_CameraRotation.y < 0) m_CameraRotation.y = 360;
        if (m_CameraRotation.y > 360) m_CameraRotation.y = 0;

        m_CameraDirection = Quaternion.Euler(m_CameraRotation) * -Vector3.forward * m_CameraDistance;

        OnCollision = Physics.Raycast(point, m_CameraDirection, out RaycastHit hit, m_CameraDistance, m_CollisionMask);
        holder.CameraTransform.position = OnCollision ? hit.point : point + m_CameraDirection;

        holder.CameraTransform.LookAt(point);
    }
#endregion

#region Collision
    [Header("Camera Collision")]
    [SerializeField] bool OnCollision;
    [SerializeField] LayerMask m_CollisionMask;
#endregion

#region LooksUtils
void RotateTowardsCameraDirection()
{
        var cameraRotation = holder.CameraTransform.eulerAngles.y;
        var direction = Quaternion.Euler(0,cameraRotation,0);

        rb.MoveRotation(direction);
}

void RotateTowardsMoveDirection()
{
    float rot = Quaternion.LookRotation(rb.linearVelocity).eulerAngles.y;
    rb.MoveRotation(Quaternion.Euler(0,rot,0));
}
#endregion
}
