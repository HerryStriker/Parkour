using System;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

#region MOVEMENT
    [Header("MOVEMENT")]
    [SerializeField, Range(0,4)] float walk_velocity;
    [SerializeField, Range(4,8)] float run_velocity;
    [SerializeField, Range(8,12)] float sprint_velocity;

    public float NormalizedVelocity
    {
        get {
            bool isMoving = InputManager.Instance.IsMoving;
            bool isSpriting = isMoving && InputManager.Instance.IsSpriting;
            bool isWalking = isMoving && InputManager.Instance.IsWalking; 
            float velocity = isWalking ? walk_velocity : isSpriting ? sprint_velocity : isMoving ? run_velocity : default; 
            //Debug.Log("Velocity: " + velocity);
            return velocity / sprint_velocity;
        }
    }

    public float Velocity {
        get {
            return Mathf.Lerp(0f, sprint_velocity, NormalizedVelocity);
        }
    }
#endregion

#region COLLISION CHECK

    [Header("CHECK")]
    [field: SerializeField] public CollisionCheck FrontCheck { get; private set; }
    [field: SerializeField] public CollisionCheck GroundCheck { get; private set; }
    [field: SerializeField] public CollisionCheck CheckPointCheck { get; private set; }

#endregion

    private void OnDrawGizmos()
    {
        GroundCheck.DrawGizmos();
        FrontCheck.DrawGizmos();
        CheckPointCheck.DrawGizmos();
    }
}