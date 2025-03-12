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
        [SerializeField] private GameObject cameraFlash;
        [SerializeField] private GameObject ultimateCamera;
        [SerializeField] private GameObject shootUltimate;
        [SerializeField] private GameObject cameraUltFlash;
        public int ammo = 3;
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
            CharacterUltimate();
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
            if (_playerLocomotionInput.ShootPressed && ammo > 0)
            {
                shootProjectile.gameObject.SetActive(true);
                cameraFlash.gameObject.SetActive(true);
                StartCoroutine(DisableShootProjectile());
                ammo--;
            }
        }

        private void CharacterUltimate()
        {
            if (_playerLocomotionInput.UltPressed && ammo == 3)
            {
                ultimateCamera.gameObject.SetActive(true);
                shootUltimate.gameObject.SetActive(true);
                cameraUltFlash.gameObject.SetActive(true);
                StartCoroutine(DisableUltimate());
                ammo = 0;
            }
        }

        private IEnumerator DisableShootProjectile()
        {
            yield return new WaitForSeconds(0.3f);
            shootProjectile.gameObject.SetActive(false);
            cameraFlash.gameObject.SetActive(false) ;
        }

        private IEnumerator DisableUltimate()
        {
            yield return new WaitForSeconds(0.5f);
            ultimateCamera.gameObject.SetActive(false);
            shootUltimate.gameObject.SetActive(false);
            cameraUltFlash.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ammo"))
            {
                if (ammo < 3)
                {
                    ammo = 3;
                }
            }
        }
    }
}