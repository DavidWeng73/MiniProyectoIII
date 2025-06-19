using UnityEngine;
using static FinalCharacterController.PlayerState;
using System;
using Unity.Netcode;
using System.Collections;

namespace FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController: NetworkBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] public CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingToTarget { get; private set; } = false;

        [Header("Base Movement")]
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float sprintAccelerataion = 50f;
        public float sprintSpeed = 7f;
        public float drag = 20f;
        public float gravity = 25f;
        public float jumpSpeed = 1.0f;
        public float movingThreshold = 0.01f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.25f;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        [Header("Jumpscare Settings (Player Specific)")]
        [SerializeField] private AudioClip _playerJumpscareSoundClip; 
        [SerializeField] private AudioSource _playerAudioSource; 

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private bool _isRotatingClockwise = false;
        private float _rotatingToTargetTimer = 0f;
        private float _verticalVelocity = 0f;

        private bool isDead = false; 

        public void SetDead(bool dead)
        {
            isDead = dead;
        }

        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            if (!IsOwner || isDead) return;
            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _playerCamera.gameObject.SetActive(false);
            }

        }

        public Transform GetCameraTransform()
        {
            return _playerCamera != null ? _playerCamera.transform : null;
        }

        private void UpdateMovementState()
        {
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;    
            bool isMovingLaterally = IsMovingLaterally();                                   
            bool isSprinting = _playerLocomotionInput.SprintToggleOn && isMovingLaterally; 
            bool isGrounded = IsGrounded();

            PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
            isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

            _playerState.SetPlayerMovementState(lateralState);

            //if (!isGrounded && _characterController.velocity.y > 0f)
            //{
            //    _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            //}
            //else if (!isGrounded && _characterController.velocity.y <= 0f)
            //{
            //    _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            //}
        }

        private void HandleVerticalMovement()
        {
            bool isGrounded = _playerState.InGroundedState();

            if (isGrounded && _verticalVelocity < 0)
                _verticalVelocity = 0f;

            _verticalVelocity -= gravity * Time.deltaTime;

            //if (_playerLocomotionInput.JumpPressed && isGrounded)
            //{
            //    _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
            //}
        }

        private void HandleLateralMovement()
        {
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = _playerState.InGroundedState();

            float lateralAcceleration = isSprinting ? sprintAccelerataion : runAcceleration;
            float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed;

            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);
            newVelocity.y += _verticalVelocity;

            _characterController.Move(newVelocity * Time.deltaTime);
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;

            float rotationTolerance = 90f;
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            IsRotatingToTarget = _rotatingToTargetTimer > 0;

            if (!isIdling)
            {
                RotatePlayerToTarget();
            }
            else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
            {
                UpdateIdleRotation(rotationTolerance);
            }

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

            Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
            float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
            RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
        }
        private void UpdateIdleRotation(float rotationTolerance)
        {
            if (Mathf.Abs(RotationMismatch) > rotationTolerance)
            {
                _rotatingToTargetTimer = rotateToTargetTime;
                _isRotatingClockwise = RotationMismatch > rotationTolerance;
            }
            _rotatingToTargetTimer -= Time.deltaTime;

            if (_isRotatingClockwise && RotationMismatch > 0f ||
                !_isRotatingClockwise && RotationMismatch < 0f)
            {
                RotatePlayerToTarget();
            }
        }

        private void RotatePlayerToTarget()
        {
            Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
        }
        #endregion



        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);

            return lateralVelocity.magnitude > movingThreshold;
        }

        private bool IsGrounded()
        {
            return _characterController.isGrounded;
        }
        #endregion

        [ClientRpc]
        public void ActivateJumpscareClientRpc()
        {
            if (!IsOwner) return;

            Debug.Log($"[PlayerController] Jumpscare activado para el cliente LOCAL con ID: {OwnerClientId}");

            SetDead(true);
            if (_playerCamera != null) _playerCamera.gameObject.SetActive(false);

            if (_playerAudioSource != null && _playerJumpscareSoundClip != null)
            {
                _playerAudioSource.PlayOneShot(_playerJumpscareSoundClip);
            }

            StartCoroutine(DeactivateJumpscareAfterDelay(5f)); 
        }

        private IEnumerator DeactivateJumpscareAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
        }
    }
}