using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Holder : MonoBehaviour
{
    Camera mainCamera;

    public Transform CameraTransform { get; private set; }
    void Awake()
    {
        GroundCheck = new CollisionCheck(GroundCheck, transform);
        FrontCheck = new CollisionCheck(FrontCheck, transform);
        CheckPointCheck = new CollisionCheck(CheckPointCheck, transform);
        
        mainCamera = Camera.main;
        CameraTransform = mainCamera.transform;
    }

    void Update()
    {
        //CheckGrounded();
        //FrontCheck();
    }

#region Collision Check

    [field: SerializeField] public CollisionCheck FrontCheck { get; private set; }
    [field: SerializeField] public CollisionCheck GroundCheck { get; private set; }

    [field: SerializeField] public CollisionCheck CheckPointCheck { get; private set; }

#endregion

    private void OnDrawGizmos()
    {
        GroundCheck.DrawGizmos();
        FrontCheck.DrawGizmos();
        CheckPointCheck.DrawGizmos();

        if(true) return;

        // var groundedColor = IsGrounded ? Color.green : Color.yellow;
        // Gizmos.color = groundedColor;
        // Gizmos.DrawSphere(transform.up * groundedHeight + transform.position, groundedRadius);

        // var frontColor = IsFrontCollision ? Color.green : Color.yellow;
        // Gizmos.color = frontColor;
        // Gizmos.DrawSphere(transform.forward * frontRange + (transform.position + transform.up), frontRadius);
        // Gizmos.DrawRay(transform.position + transform.up, transform.forward * frontRange);

    }
}