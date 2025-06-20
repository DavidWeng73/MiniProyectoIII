using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace FinalCharacterController
{
    public class PlayerAnimation : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerController _playerController;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");
        private static int isAimingHash = Animator.StringToHash("isAiming");
        private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        private Vector3 _currentBlendInput = Vector3.zero;

        private NetworkAnimator _networkAnimator;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _networkAnimator = GetComponent<NetworkAnimator>();
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isAiming = _playerState.CurrentPlayerMovementState == PlayerMovementState.Aiming;
            bool isGrounded = _playerState.InGroundedState();

            Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * 1.5f :
                                  isRunning ? _playerLocomotionInput.MovementInput * 1f : _playerLocomotionInput.MovementInput * 0.5f;

            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _networkAnimator.Animator.SetBool(isGroundedHash, isGrounded);
            _networkAnimator.Animator.SetBool(isIdlingHash, isIdling);
            _networkAnimator.Animator.SetBool(isFallingHash, isFalling);
            _networkAnimator.Animator.SetBool(isJumpingHash, isJumping);
            _networkAnimator.Animator.SetBool(isAimingHash, isAiming);
            _networkAnimator.Animator.SetBool(isRotatingToTargetHash, _playerController.IsRotatingToTarget);

            _networkAnimator.Animator.SetFloat(inputXHash, _currentBlendInput.x);
            _networkAnimator.Animator.SetFloat(inputYHash, _currentBlendInput.y);
            _networkAnimator.Animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _networkAnimator.Animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }
    }
}

