using FinalCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControl.IPlayerLocomotionMapActions
    {
        #region Class Variables
        [SerializeField] private bool holdToSprint = true;
        [SerializeField] private bool holdToAim = true;
        public bool  SprintToggleOn {  get; private set; }
        public PlayerControl PlayerControl { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool AimPressed { get; private set; }
        public bool ShootPressed { get; private set; }
        #endregion

        #region Startup
        private void OnEnable()
        {
            PlayerControl = new PlayerControl();
            PlayerControl.Enable();

            PlayerControl.PlayerLocomotionMap.Enable();
            PlayerControl.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControl.PlayerLocomotionMap.Disable();
            PlayerControl.PlayerLocomotionMap.RemoveCallbacks(this);
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            JumpPressed = false;
        }
        #endregion

        #region Input Callbacks
        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggleOn = holdToSprint || !SprintToggleOn;
            }
            else if (context.canceled)
            {
                SprintToggleOn = !holdToSprint && SprintToggleOn;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            JumpPressed = true;
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            ShootPressed = true;
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AimPressed = holdToAim || !AimPressed;
            }
            else if (context.canceled)
            {
                AimPressed = !holdToAim && AimPressed;
            }
            Debug.Log("AimPressed: " + AimPressed);
        }
        #endregion
    }
}
