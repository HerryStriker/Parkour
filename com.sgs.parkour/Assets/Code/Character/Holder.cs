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
        _locomotion = GetComponent<Locomotion>();
        GroundCheck = new CollisionCheck(GroundCheck, transform);
        FrontCheck = new CollisionCheck(FrontCheck, transform);
        CheckPointCheck = new CollisionCheck(CheckPointCheck, transform);
        
        mainCamera = Camera.main;
        CameraTransform = mainCamera.transform;
    }

    void Update()
    {
        Timer();
    }

    #region JUMP CONTROL
    Locomotion _locomotion;
    public bool InAir {get; set;}


    public float CurrentTime { get; private set; }
    public float NormalizedTime 
    {
        get
        {
            return CurrentTime / MAX_TIME;
        }
    }
    const float MIN_TIME = 0;
    const float MAX_TIME = 3;

    void Timer()
    {
        // CAN JUMP IF IS ON GROUND
        var canJump = _locomotion.IsJumping && CurrentTime < MAX_TIME && GroundCheck.IsColliding;

        //CAN JUMP IF IS ON AIR AND HAVE DOUBLE JUMP LEFT
        var canDoubleJump = InputManager.Instance.IsJumping && _locomotion.IsJumping && CurrentTime < MAX_TIME && !GroundCheck.IsColliding && _locomotion.JumpCount > 0;

        CurrentTime += canJump || canDoubleJump ? Time.deltaTime : -Time.deltaTime;
        CurrentTime = Mathf.Clamp(CurrentTime, MIN_TIME, MAX_TIME);
    }

    public void ResetTime()
    {
        CurrentTime = 0;
    }
#endregion

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