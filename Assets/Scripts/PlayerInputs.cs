using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour, InputSystem_Actions.IPlayerActions
{

    public InputSystem_Actions InputActions { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool ExitInput { get; private set; }
    public bool RadioInput { get; private set; }
    public static event Action ExitFunc;
    //public static event Action<bool> onWalkieCLick;
    //public bool IsInteracting = false;

    private void OnEnable()
    {
        InputActions = new InputSystem_Actions();

        InputActions.Player.Enable();
        InputActions.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        InputActions.Player.Disable();
        InputActions.Player.RemoveCallbacks(this);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InteractInput = context.ReadValueAsButton() && !InteractInput;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //if(!IsInteracting)
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        //IsInteracting = false;
        ExitInput = context.ReadValueAsButton() && !ExitInput;
       
        ExitFunc?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintInput = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackInput = context.ReadValueAsButton();
    }

   
}
