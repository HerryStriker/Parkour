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

    void IPlayerActions.OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
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
    void IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }
    public void EnableMove()
    {
        inputActions.Player.Move.Enable();
    }
    
    public void DisableMove()
    {
        inputActions.Player.Move.Disable();
    }

    void IPlayerActions.OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void IPlayerActions.OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void IPlayerActions.OnSprint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

}

