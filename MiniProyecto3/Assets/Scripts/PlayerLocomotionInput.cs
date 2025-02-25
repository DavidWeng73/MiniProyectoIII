using FinalCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionInput : MonoBehaviour, PlayerControl.IPlayerLocomotionMapActions
{
    public PlayerControl PlayerControl {  get; private set; }
    public Vector2 MovementInput { get; private set; }


    private void OnEnable()
    {
        PlayerControl = new PlayerControl();
        PlayerControl.Enable();

        PlayerControl.PlayerLocomotionMap.Enable();
        PlayerControl.PlayerLocomotionMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        PlayerControl.PlayerLocomotionMap.Enable();
        PlayerControl.PlayerLocomotionMap.SetCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        print (MovementInput);
    }
}
