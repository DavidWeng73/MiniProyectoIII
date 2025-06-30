using UnityEngine;
using Cinemachine;
using static FinalCharacterController.PlayerState;
using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Windows;

namespace FinalCharacterController
{
    public class ThirdPersonShooterController : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
        [SerializeField] private GameObject shootProjectile;
        [SerializeField] private GameObject cameraFlash;
        [SerializeField] private GameObject ultimateCamera;
        [SerializeField] private GameObject shootUltimate;
        [SerializeField] private GameObject cameraUltFlash;
        [SerializeField] private GameObject batteryUIRoot;
        public int ammo = 3;
        public GameObject battery;
        public GameObject Bigbattery;
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private Animator animator;
        public Image[] batteryIcons;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip ultimateClip;
        private PlayerController playerController;
        [SerializeField] private GameObject playerfreezeEffect;

        //[SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            animator = GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();
        }
        private void Update()
        {
            if (!IsOwner) return;

            if (_playerLocomotionInput.ShootPressed && ammo > 0)
            {
                Debug.Log("Intentando llamar ServerRpc desde cliente: " + IsOwner);
                Debug.Log("[CLIENT] Disparo intentado");
                RequestShootServerRpc();
            }

            if (PauseMenu.isPaused) return;

            AimCameraRotation();
            CharacterUltimate();
        }

        private void Start()
        {
            if (IsOwner && playerController != null)
            {
                aimVirtualCamera.Follow = playerController.GetCameraTransform();
                aimVirtualCamera.LookAt = playerController.GetCameraTransform();
            }
        }


        private void UpdateBatteryUI()
        {
            for (int i = 0; i < batteryIcons.Length; i++)
            {
                batteryIcons[i].enabled = i < ammo;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner && batteryUIRoot != null)
            {
                batteryUIRoot.SetActive(false); 
            }
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

        //private void CharacterShoot()
        //{
        //    Debug.Log($"[CharacterShoot] ShootPressed={_playerLocomotionInput.ShootPressed}, ammo={ammo}");
        //    if (_playerLocomotionInput.ShootPressed && ammo > 0)
        //    {
        //        Debug.Log("[CharacterShoot] Llamando a ShootFlashClientRpc");
        //        ShootFlashClientRpc();
        //        audioSource.PlayOneShot(shootClip);
        //        ammo--;
        //        UpdateBatteryUI();
        //    }
        //}

        private void CharacterUltimate()
        {
            if (_playerLocomotionInput.UltPressed && ammo == 3)
            {
                ultimateCamera.gameObject.SetActive(true);
                shootUltimate.gameObject.SetActive(true);
                cameraUltFlash.gameObject.SetActive(true);
                audioSource.PlayOneShot(ultimateClip);
                StartCoroutine(DisableUltimate());
                ammo = 0;
                RequestUltimateFreezeServerRpc(transform.position + transform.forward * 5f);
                UpdateBatteryUI();
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

        //[ClientRpc]
        //void ShootFlashClientRpc()
        //{
        //    Debug.Log($"[ShootFlashClientRpc] recibido en {gameObject.name}, IsOwner={IsOwner}");

        //    if (cameraFlash == null)
        //    {
        //        return;
        //    }

        //    cameraFlash.SetActive(true);
        //    Debug.Log("[ShootFlashClientRpc] Flash activado.");
        //    StartCoroutine(DisableShootFlash());
        //}

        //[ServerRpc]
        [ServerRpc(RequireOwnership = false)]
        private void RequestShootServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("¡ServerRpc ejecutado en el servidor!");
            Debug.Log("[SERVER] ServerRpc ejecutado");
            ammo--;
            ShowFlashClientRpc();
            PlayShootSoundClientRpc();
            UpdateAmmoClientRpc(ammo);

            Ray ray = new Ray(projectileSpawnPoint.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                Debug.Log($"[SERVER] Raycast hit: {hit.collider.name}, tag: {hit.collider.tag}");

                if (hit.collider.CompareTag("Paints"))
                {
                    hit.collider.GetComponent<Paint>().DestroyPaint();
                }
                else if (hit.collider.CompareTag("FakePaints"))
                {
                    var paint = hit.collider.GetComponent<Paint>();
                    if (paint != null)
                    {
                        paint.FakePaintTrap(rpcParams.Receive.SenderClientId);
                    }
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    var enemy = hit.collider.GetComponent<EnemyAI>();
                    if (enemy != null)
                    {
                        enemy.FreezeServerRpc(3f); // Añadido: congelar enemigo por 3 segundos
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestUltimateFreezeServerRpc(Vector3 center)
        {
            Collider[] hits = Physics.OverlapSphere(center, 7f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    var enemy = hit.GetComponent<EnemyAI>();
                    if (enemy != null)
                    {
                        enemy.FreezeServerRpc(6f); // Congelar enemigo por 6 segundos
                    }
                }
            }
        }

        [ClientRpc]
        private void ShowFlashClientRpc()
        {
            if (cameraFlash != null)
            {
                cameraFlash.SetActive(true);
                Invoke(nameof(HideFlash), 0.3f);
            }
        }

        [ClientRpc]
        private void PlayShootSoundClientRpc()
        {
            audioSource.PlayOneShot(shootClip);
        }

        [ClientRpc]
        private void UpdateAmmoClientRpc(int newAmmo)
        {
            ammo = newAmmo;
            UpdateBatteryUI();
        }

        private void HideFlash()
        {
            cameraFlash.SetActive(false);
        }

        //private IEnumerator DisableShootFlash()
        //{
        //    yield return new WaitForSeconds(0.3f);
        //    cameraFlash.SetActive(false);
        //}

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ammo"))
            {
                if (ammo < 3)
                {
                    ammo++;
                    UpdateBatteryUI();
                    battery.gameObject.SetActive(false);
                }
            }

            if (other.CompareTag("BigAmmo"))
            {
                if (ammo < 3)
                {
                    ammo = 3;
                    UpdateBatteryUI();
                    Bigbattery.gameObject.SetActive(false);
                }
            }
        }

        public void TriggerFreezeFeedback(float duration)
        {
            StartCoroutine(FreezeFeedbackRoutine(duration));
        }

        private IEnumerator FreezeFeedbackRoutine(float duration)
        {
            // Bloquear movimiento
            var controller = GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            // Activar efecto
            if (playerfreezeEffect != null) playerfreezeEffect.SetActive(true);

            yield return new WaitForSeconds(duration);

            // Desactivar efecto
            if (playerfreezeEffect != null) playerfreezeEffect.SetActive(false);

            if (controller != null) controller.enabled = true;
        }

    }
}

