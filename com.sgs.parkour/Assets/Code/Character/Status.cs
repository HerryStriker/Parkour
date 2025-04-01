using System;
using UnityEngine;

public class Status : MonoBehaviour
{
    Holder holder;
    Locomotion _locomotion;
    ModeInteraction _modeInteraction;
    void Awake()
    {
        _modeInteraction = GetComponent<ModeInteraction>();
        holder = GetComponent<Holder>();
        _locomotion = GetComponent<Locomotion>();
    }

    void Start()
    {
        doubleJumpCounter = new DoubleJumpCounter(doubleJumpCounter, _locomotion.JumpCount);
        loaderController = new LoaderController(loaderController);
        lifeController = new LifeController(lifeController);
        checkPointInteraction = new InteractionDisplay(checkPointInteraction);

        _modeInteraction.OnCheckPointEnter += OnEnterCheckPoint;
        holder.CheckPoint_Check.OnExit += OnExitCheckPoint;
        _modeInteraction.OnGetCheckPointCallback += OnGetCheckPoint;    

        deathController = new DeathController(deathController);
        deathController.OnClickRespawnCallback += OnRespawnClick;

        InputManager.Instance.OnJumpStart += OnJumpStart;
        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;

        _locomotion.OnDoubleJumpChangedCallback += OnDoubleJumpChanged;
        
        holder.OnDeath += OnDeath;
    }

    private void OnGetCheckPoint()
    {
        checkPointInteraction.Enable(false);        
    }

    private void OnExitCheckPoint(object sender, EventArgs e)
    {
        checkPointInteraction.Enable(false);        
    }

    private void OnEnterCheckPoint()
    {
        checkPointInteraction.Enable(true);
    }

    private void OnDeath(float arg1, float arg2, float arg3)
    {
        deathController.Show();
    }

    private void OnRespawnClick(object sender, EventArgs e)
    {
        ModeManager modeManager = Utils.GetModeManager(); 
        MatchScoreManager matchScoreManager = Utils.GetMatchScoreManager(); 

        if(RushMode.Instance != null)
        {

        }


        holder.Respawn(_modeInteraction.LastestCheckPointPosition);

        deathController.Hide();
        _locomotion.EnableCharacterMovement(true);
    }

    private void OnDoubleJumpChanged(object sender, EventArgs e)
    {
        doubleJumpCounter.ChangeValue(_locomotion.JumpCount);
    }

    void Update()
    {
        loaderController?.Update(holder.NormalizedTime);
        lifeController?.Update(holder.Life.NormalizedValue);
    }

    private void OnJumpCanceled(object sender, EventArgs e)
    {
        loaderController.Disable();
    }

    private void OnJumpStart(object sender, EventArgs e)
    {
        loaderController.Enable();
    }



    // JUMP LOADER
    [SerializeField] LoaderController loaderController;

    // DOUBLE JUMP COUNTER
    [SerializeField] DoubleJumpCounter doubleJumpCounter;

    // LIFE
    [SerializeField] LifeController lifeController;

    // DEATH
    [SerializeField] DeathController deathController;
     
    [SerializeField] InteractionDisplay checkPointInteraction;
}
