using System;
using UnityEngine;

public class Status : MonoBehaviour
{
    Holder holder;
    Locomotion _locomotion;
    void Awake()
    {
        holder = GetComponent<Holder>();
        _locomotion = GetComponent<Locomotion>();
    }

    void Start()
    {
        doubleJumpCounter = new DoubleJumpCounter(doubleJumpCounter, _locomotion.JumpCount);
        loaderController = new LoaderController(loaderController);
        lifeController = new LifeController(lifeController);

        InputManager.Instance.OnJumpStart += OnJumpStart;
        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;

        _locomotion.OnDoubleJumpChangedCallback += OnDoubleJumpChanged;
        
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
     
}
