using FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : NetworkBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, sightDistance, catchDistance, chaseTime, minChaseTime, maxChaseTime, jumpscareTime;
    public bool walking, chasing;
    //public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum;
    public int destinationAmount;
    public Vector3 rayCastOffset;
    public string deathScene;
    public NetworkVariable<bool> isChasing = new NetworkVariable<bool>();
    public NetworkVariable<Vector3> networkedDestination = new NetworkVariable<Vector3>();

    void Start()
    {
        walking = true;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }
    void Update()
    {
        if (!IsServer) return;
        Transform targetPlayer = GetClosestPlayer();
        if (targetPlayer == null) return;
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, sightDistance))
        {
            if (hit.collider.CompareTag ("Player"))
            {
                walking = false;
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                StartCoroutine("chaseRoutine");
                isChasing.Value = true;
                networkedDestination.Value = targetPlayer.position;
                chasing = true;
                SetAnimationClientRpc("sprint");
            }
        }
        if (chasing == true)
        {
            dest = targetPlayer.position;
            ai.destination = dest;
            networkedDestination.Value = dest;
            ai.speed = chaseSpeed;
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.SetTrigger("sprint");
            float distance = Vector3.Distance(targetPlayer.position, ai.transform.position);
            if (distance <= catchDistance)
            {
                aiAnim.ResetTrigger("walk");
                aiAnim.ResetTrigger("idle");
                aiAnim.ResetTrigger("sprint");
                //aiAnim.SetTrigger("jumpscare");

                var networkObject = targetPlayer.GetComponent<NetworkObject>();
                var playerController = targetPlayer.GetComponent<PlayerController>();

                if (networkObject != null)
                {
                    var clientId = networkObject.OwnerClientId;

                    ClientRpcParams clientRpcParams = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { clientId } 
                        }
                    };

                    SetAnimationClientRpc("jumpscare", clientRpcParams);

                    if (playerController != null)
                    {
                        playerController.ActivateJumpscareClientRpc();
                    }

                    DisablePlayerClientRpc(clientId);
                    StartCoroutine(JumpscareAndRespawn(targetPlayer, clientId));
                }

                chasing = false;
            }
        }
        if (walking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            aiAnim.ResetTrigger("sprint");
            aiAnim.ResetTrigger("idle");
            aiAnim.SetTrigger("walk");
            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                aiAnim.ResetTrigger("sprint");
                aiAnim.ResetTrigger("walk");
                aiAnim.SetTrigger("idle");
                ai.speed = 0;
                StopCoroutine("stayIdle");
                StartCoroutine("stayIdle");
                walking = false;
            }
        }
    }
    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }
    IEnumerator chaseRoutine()
    {
        chaseTime = Random.Range(minChaseTime, maxChaseTime);
        yield return new WaitForSeconds(chaseTime);
        walking = true;
        chasing = false;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }
    //IEnumerator deathRoutine()
    //{
    //    yield return new WaitForSeconds(jumpscareTime);
    //}

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareClip;

    public void PlayJumpscareSound()
    {
        if (audioSource && jumpscareClip)
        {
            audioSource.PlayOneShot(jumpscareClip);
        }
    }

    IEnumerator JumpscareAndRespawn(Transform player, ulong clientId)
    {
        yield return new WaitForSeconds(jumpscareTime);

        var respawn = player.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.RespawnServerRpc(clientId);
            Transform spawnPoint = RespawnManager.Instance.GetSpawnPoint(SceneManager.GetActiveScene().name);
            if (spawnPoint != null)
            {
                respawn.OnRespawnClientRpc(spawnPoint.position);
            }
        }

        StartCoroutine(TemporarilyIgnorePlayer(player.gameObject));

        aiAnim.ResetTrigger("jumpscare");
        aiAnim.ResetTrigger("sprint");
        aiAnim.ResetTrigger("idle");
        aiAnim.ResetTrigger("walk");
        SetAnimationClientRpc("idle");

        walking = true;
        chasing = false;
        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }

    private HashSet<GameObject> temporarilyIgnored = new HashSet<GameObject>();

    IEnumerator TemporarilyIgnorePlayer(GameObject player)
    {
        temporarilyIgnored.Add(player);
        yield return new WaitForSeconds(3f); 
        temporarilyIgnored.Remove(player);
    }

    [ClientRpc]
    void SetAnimationClientRpc(string animation, ClientRpcParams clientRpcParams = default)
    {
        aiAnim.ResetTrigger("walk");
        aiAnim.ResetTrigger("idle");
        aiAnim.ResetTrigger("sprint");
        aiAnim.ResetTrigger("jumpscare");
        aiAnim.SetTrigger(animation);
    }

    private Transform GetClosestPlayer()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null && !temporarilyIgnored.Contains(playerObject.gameObject))
            {
                float distance = Vector3.Distance(transform.position, playerObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = playerObject.transform;
                }
            }
        }
        return closestPlayer;
    }

    //[ClientRpc]
    //void PlayerCaughtClientRpc()
    //{
    //    if (IsOwner)
    //    {
    //        gameObject.SetActive(false); 
    //    }
    //}
    
    [ClientRpc]
    void DisablePlayerClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var playerController = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
            var camera = playerController.GetCameraTransform()?.gameObject;

            if (camera != null) camera.SetActive(false);
            playerController.SetDead(true) ;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FreezeServerRpc(float duration)
    {
        if (isFrozen) return;

        StartCoroutine(FreezeRoutine(duration));
    }

    private bool isFrozen = false;

    private IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        float originalSpeed = ai.speed;

        ai.speed = 0;
        ai.isStopped = true;
        aiAnim.SetTrigger("idle");

        yield return new WaitForSeconds(duration);

        ai.isStopped = false;
        isFrozen = false;

        // Volver al estado previo
        if (chasing)
        {
            ai.speed = chaseSpeed;
            aiAnim.SetTrigger("sprint");
        }
        else
        {
            ai.speed = walkSpeed;
            aiAnim.SetTrigger("walk");
        }
    }
}