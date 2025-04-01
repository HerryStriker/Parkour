using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputActions;

public class InputManager : MonoBehaviour, IPlayerActions
{
    public static InputManager Instance { get; private set;}

    InputActions inputActions;
    
    private void Awake()
    {
        Instance = this;

        inputActions = new InputActions();
        inputActions.Player.SetCallbacks(this);
    }

    void IPlayerActions.OnAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void IPlayerActions.OnCrouch(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void EnableInteract()
    {
        inputActions.Player.Interact.Enable();
    }

    public void DisableInteract()
    {
        inputActions.Player.Interact.Disable();
    }

    public event EventHandler OnInteractStart;
    public event EventHandler OnInteractPerformed;
    public event EventHandler OnInteractCanceled;

    public bool IsInteracting { get; private set; }
    void IPlayerActions.OnInteract(InputAction.CallbackContext context)
    {
        InvokeEvents_SPC(context, OnInteractStart, OnInteractPerformed, OnInteractCanceled);
        IsInteracting = context.ReadValue<float>() > 0;
    }

    public void EnableJump() {
        inputActions.Player.Jump.Enable();
    }
    
    public void DsableJump() {
        inputActions.Player.Jump.Disable();
    }


    public event EventHandler OnJumpStart;
    public event EventHandler OnJumpPerformed;
    public event EventHandler OnJumpCanceled;

    public bool IsJumping { get; private set;}

    void IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        switch(context.phase) 
        {
            case InputActionPhase.Started:
                OnJumpStart?.Invoke(this, EventArgs.Empty);
            break;

            case InputActionPhase.Performed:
                OnJumpPerformed?.Invoke(this, EventArgs.Empty);
            break;

            case InputActionPhase.Canceled:
                OnJumpCanceled?.Invoke(this, EventArgs.Empty);
            break;
        }
        IsJumping = context.ReadValue<float>() > 0;
    }
    
    public Vector2 Look {get; private set;}
    void IPlayerActions.OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    public void EnableLook() {
        inputActions.Player.Look.Enable();
    }
    
    public void DisableLook() {
        inputActions.Player.Look.Disable();
    }

    public Vector2 Move {get; private set;}
    public bool IsMoving {get; private set;}
    void IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>().normalized;
        IsMoving = Move.magnitude > 0;
    }
    public void EnableMove()
    {
        inputActions.Player.Move.Enable();
        inputActions.Player.Walk.Enable();
        inputActions.Player.Sprint.Enable();

    }
    
    public void DisableMove()
    {
        inputActions.Player.Move.Disable();
        inputActions.Player.Walk.Disable();
        inputActions.Player.Sprint.Disable();
    }

    void IPlayerActions.OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void IPlayerActions.OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public event EventHandler OnSprintStart;
    public event EventHandler OnSprintPerformed;
    public event EventHandler OnSprintCanceled;

    public bool IsSpriting {get; private set;}

    void IPlayerActions.OnSprint(InputAction.CallbackContext context)
    {
        IsSpriting = context.ReadValue<float>() > 0;
        InvokeEvents_SPC(context, OnSprintStart, OnSprintPerformed, OnSprintCanceled);
    }

    public void EnableCancelAction() {
        inputActions.Player.CancelAction.Enable();
    }
    
    public void DisableCancelAction() {
        inputActions.Player.CancelAction.Disable();
    }

    public event EventHandler OnCancel;

    public void OnCancelAction(InputAction.CallbackContext context)
    {
        switch (context.phase) 
        {
            case InputActionPhase.Started:
                OnCancel?.Invoke(this, EventArgs.Empty);
            break;
        }
    }
    public event EventHandler OnWalkStart;
    public event EventHandler OnWalkPerformed;
    public event EventHandler OnWalkCanceled;

    public bool IsWalking {get; private set;}

    public void OnWalk(InputAction.CallbackContext context)
    {
        IsWalking = context.ReadValue<float>() > 0 ;
        InvokeEvents_SPC(context, OnWalkStart, OnWalkPerformed, OnWalkCanceled);
    }

    void InvokeEvents_SPC(InputAction.CallbackContext ctx, params EventHandler[] events)
    {
        switch(ctx.phase) 
        {
            case InputActionPhase.Started:
                events[0]?.Invoke(this, EventArgs.Empty);
            break;

            case InputActionPhase.Performed:
                events[1]?.Invoke(this, EventArgs.Empty);
            break;

            case InputActionPhase.Canceled:
                events[2]?.Invoke(this, EventArgs.Empty);
            break;
        }   
        
    }
}