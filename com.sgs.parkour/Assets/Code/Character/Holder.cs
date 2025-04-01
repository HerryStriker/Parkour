using System;
using UnityEngine;
using Parkour.Status;
using System.Collections.Generic;

public class Holder : MonoBehaviour
{
    Camera mainCamera;

    public Transform CameraTransform { get; private set; }

    #region Ragdoll
    [SerializeField] CapsuleCollider characterCollider;

    [SerializeField] RagdollController ragdollController;
        
    #endregion


    #region COLLISION CHECK

    [field: SerializeField] public CollisionCheck FrontCheck { get; private set; }
    [field: SerializeField] public CollisionCheck GroundCheck { get; private set; }
    [field: SerializeField] public CollisionCheckData<CheckPoint> CheckPoint_Check { get; private set; }

    public bool EqualCheckPoint(CheckPoint checkPoint)
    {
        var checkdata = CheckPoint_Check.GetData();

        if(checkPoint != null && checkdata != null)
        {
            return checkPoint.FlagID == checkdata.FlagID;
        }
        return false;
    }

    #endregion
    
    #region LIFE
    /// <summary>
    /// Player take damage
    /// </summary>
    public event Action<float, float, float> OnLoseLife;

    /// <summary>
    /// Player heal max life
    /// </summary>
    public event Action<float, float, float> OnMaxLife;

    /// <summary>
    /// Player die
    /// </summary>
    public event Action<float, float, float> OnDeath;

    [field: SerializeField] public bool IsAlive {get; private set;} = true;

    [field: SerializeField] public UnitStatus Life {get; private set;} 

    private void Life_OnValueChangedCallback(object sender, EventArgs e)
    {
        OnLoseLife?.Invoke(Life.CurrentValue, Life.MaxValue, Life.NormalizedValue);
    }

    private void Life_OnValueEqualToMax(object sender, EventArgs e)
    {
        OnMaxLife?.Invoke(Life.CurrentValue, Life.MaxValue, Life.NormalizedValue);
    }

    private void Life_OnValueEqualToZero(object sender, EventArgs e)
    {
        OnDeath?.Invoke(Life.CurrentValue, Life.MaxValue, Life.NormalizedValue);
    }

    const float MAX_LIFE = 1000f;

    [SerializeField] float fallingTime;
    const float INSTANT_FALLING_DEAD_TIME = 6f;
    const float MAX_FALLING_TIME = 3f;
    const float MIN_FALLING_TIME = 1f; 

    // DAMAGE BY FALLING
    const float MIN_FALLING_DAMAGE = 100f;
    const float MAX_FALLING_DAMAGE = MAX_LIFE;

        const float REVIVE_LIFE = .2f;
        const float RESPAWN_LIFE = 1f;
    public void Revive(float lifePercentage)
    {
        ragdollController.EnableRagdoll(false);
        Life.ChangePercentageValue(lifePercentage, ValueType.INCREASE);
        IsAlive = true;
        fallingTime = 0;
    }

    public void Respawn(Vector3 respawnPosition)
    {
        _locomotion.MovePosition(respawnPosition);
        Revive(RESPAWN_LIFE);
    }

    void LifeLogic()
    {
        if(_locomotion.IsFalling)
        {
            fallingTime += Time.deltaTime;
            fallingTime = Mathf.Clamp(fallingTime, 0, INSTANT_FALLING_DEAD_TIME);
        }

        if(fallingTime >= INSTANT_FALLING_DEAD_TIME && IsAlive)
        {
            OnDeath?.Invoke(default, default, default);
        }    
    }

    void ApplyFallDamageLogic()
    {
        float normalizedFallingTime = fallingTime / MAX_FALLING_TIME;

        float normalizedDamageTake = Mathf.Lerp(MIN_FALLING_DAMAGE, MAX_FALLING_DAMAGE, normalizedFallingTime);

        if(fallingTime >= MIN_FALLING_TIME)
        {
            var normalizedLifeValue = normalizedDamageTake * normalizedFallingTime;
            Life.ChangeValue(normalizedLifeValue, ValueType.DECREASE);
            // Debug.Log("Damage taked: " + Mathf.Abs(normalizedLifeValue));
        }

        fallingTime = 0;
    }
    
    private void Life_OnDeath(float arg1, float arg2, float arg3)
    {
        ragdollController.EnableRagdoll(true, _locomotion.IsFalling);
        IsAlive = false;
        _locomotion.EnableCharacterMovement(false);
    }


    #endregion

    void Awake()
    {
        characterCollider = GetComponentInParent<CapsuleCollider>();
        characterCollider.center += new Vector3(0, 1, 0);
        characterCollider.height = 2;

        _locomotion = GetComponent<Locomotion>();
        GroundCheck = new CollisionCheck(GroundCheck, transform);
        FrontCheck = new CollisionCheck(FrontCheck, transform);
        CheckPoint_Check = new CollisionCheckData<CheckPoint>(CheckPoint_Check, CheckPoint_Check, transform);
        
        mainCamera = Camera.main;
        CameraTransform = mainCamera.transform;

        Life = new UnitStatus(MAX_LIFE);

        Life.OnValueChangedCallback += Life_OnValueChangedCallback;
        Life.OnValueEqualToZero += Life_OnValueEqualToZero;
        Life.OnValueEqualToMax += Life_OnValueEqualToMax;

        OnDeath += Life_OnDeath;
    }

    void Start()
    {
        GroundCheck.OnEnter += OnGroundEnter;
        
    }

    private void OnGroundEnter(object sender, EventArgs e)
    {
        ApplyFallDamageLogic();
    }

    void Update()
    {
        Timer();
        LifeLogic();
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
    [Space(15)]
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
            return Mathf.Lerp(walk_velocity, sprint_velocity, NormalizedVelocity);
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        GroundCheck.DrawGizmos();
        FrontCheck.DrawGizmos();
        CheckPoint_Check.DrawGizmos();
    }

}