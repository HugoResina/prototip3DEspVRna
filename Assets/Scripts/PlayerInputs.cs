using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public InputSystem_Actions InputActions { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void OnEnable()
    {
        InputActions = new InputSystem_Actions();
        InputActions.Enable();

        InputActions.Player.Enable();
        InputActions.Player.SetCallbacks(this);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
}
