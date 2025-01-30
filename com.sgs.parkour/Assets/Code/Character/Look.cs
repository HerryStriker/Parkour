using UnityEngine;

public class Look : MonoBehaviour
{
    Camera cam;
    Rigidbody rb;
    Transform cameraTransform;

    [Header("Camera")]
    [SerializeField] Vector3 m_CameraDirection = Vector3.zero;
    [SerializeField, Range(0, 10)] float m_CameraPointOffset = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityX = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityY = 1f;
    [SerializeField, Range(0, 10)] float m_CameraSensitivityNormalized = 2f;
    [SerializeField, Range(-180, 180)] float m_MaxRotation = 60;
    [SerializeField, Range(-180, 180)] float m_MinRotation = -60;
    [SerializeField, Range(1, 10)] float m_CameraDistance = 2.5f;
    [SerializeField] InputSetting m_CameraInputSettings;

    [Header("Rotation")]
    [SerializeField] Vector2 m_CameraInput;
    [SerializeField] Vector2 m_CameraRotation = Vector2.zero;
    [SerializeField, Range(1, 10)] float m_RotationSpeed = 4f;
    [SerializeField] bool m_EnableRotation = true;

    [Header("Camera Collision")]
    [SerializeField] bool OnCollision;
    [SerializeField] LayerMask m_CollisionMask;

    private void Awake() {
        cam = Camera.main;
        cameraTransform = cam.transform;

        rb = GetComponent<Rigidbody>();

    }


    void Start() {
        InputManager.Instance.EnableLook();
    }

    void Update() {
        CameraBehaviour();
        Rotation();
    }
    void Rotation()
    {
        rb.MoveRotation(Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0));
    }

    void CameraBehaviour()
    {
        Vector2 input = new Vector2(InputManager.Instance.Look.x * m_CameraSensitivityX, InputManager.Instance.Look.y * m_CameraSensitivityY) * m_CameraSensitivityNormalized;
        m_CameraInput = m_CameraInputSettings.GetSmoothDampSettings(input, ref m_CameraInput);
        //m_CameraInput = InputManager.Instance.MouseInput;

        Vector3 point = transform.position + Vector3.up * m_CameraPointOffset;

        //m_CameraRotation += m_CameraInput;
        m_CameraRotation += m_CameraInputSettings.GetSettings(m_CameraInput.y, m_CameraInput.x);


        m_CameraRotation.x = Mathf.Clamp(m_CameraRotation.x, m_MinRotation, m_MaxRotation);

        if (m_CameraRotation.y < 0) m_CameraRotation.y = 360;
        if (m_CameraRotation.y > 360) m_CameraRotation.y = 0;

        m_CameraDirection = Quaternion.Euler(m_CameraRotation) * -Vector3.forward * m_CameraDistance;

        OnCollision = Physics.Raycast(point, m_CameraDirection, out RaycastHit hit, m_CameraDistance, m_CollisionMask);

        cameraTransform.position = OnCollision ? hit.point : point + m_CameraDirection;

        cameraTransform.LookAt(point);
    }

}
