using UnityEngine;
using Cinemachine;
using static FinalCharacterController.PlayerState;
using System.Collections;

namespace FinalCharacterController
{
    public class ThirdPersonShooterController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
        [SerializeField] private GameObject shootProjectile;
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private Animator animator;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            AimCameraRotation();
            CharacterShoot();
        }

        private void AimCameraRotation()
        {
            if (_playerLocomotionInput.AimPressed)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            }
            else
            {
                aimVirtualCamera.gameObject.SetActive(false);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            }
        }

        private void CharacterShoot()
        {
            if (_playerLocomotionInput.ShootPressed)
            {
                shootProjectile.gameObject.SetActive(true);
                StartCoroutine(DisableShootProjectile());
            }

            else 
            {
                shootProjectile.gameObject.SetActive(false);
            }

        }

        private IEnumerator DisableShootProjectile()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}