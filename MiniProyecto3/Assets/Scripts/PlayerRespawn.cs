using UnityEngine;
using Unity.Netcode;
using FinalCharacterController;
using UnityEngine.SceneManagement;

public class PlayerRespawn : NetworkBehaviour
{
    private bool hasTeleported = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadedServer;
        }
        if (IsOwner)
        {
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadedClient;
        }
    }

    private void OnSceneLoadedServer(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        Transform spawn = RespawnManager.Instance.GetSpawnPoint(sceneName);
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        player.transform.position = spawn.position;
    }

    private void OnSceneLoadedClient(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (clientId != NetworkManager.LocalClientId) return;
        TeleportToSpawn(sceneName);
    }

    public override void OnDestroy()
    {
        if (IsOwner && NetworkManager.Singleton != null)
        {
            NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (hasTeleported) return;
        TeleportToSpawn(sceneName);
        hasTeleported = true;
    }

    private void TeleportToSpawn(string sceneName)
    {
        Transform spawnPoint = RespawnManager.Instance.GetSpawnPoint(sceneName);
        if (spawnPoint != null)
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            transform.position = spawnPoint.position;

            if (controller != null) controller.enabled = true;

            Debug.Log($"[PlayerRespawn] Teleportado al spawn de la escena: {sceneName}");
        }
        else
        {
            Debug.LogWarning("[PlayerRespawn] No se encontró punto de respawn para esta escena.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc(ulong clientId)
    {
        if (IsServer && LifeManager.Instance != null)
        {
            LifeManager.Instance.PlayerLostLife();
            Debug.Log("Vida restada");
        }

        string currentScene = SceneManager.GetActiveScene().name;
        Transform spawnPoint = RespawnManager.Instance.GetSpawnPoint(currentScene);

        if (spawnPoint != null)
        {
            var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            playerObject.transform.position = spawnPoint.position;
            playerObject.gameObject.SetActive(true);

            OnRespawnClientRpc(spawnPoint.position, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }

    [ClientRpc]
    public void OnRespawnClientRpc(Vector3 newPosition, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;  // Solo el propietario actualiza su posición y cámara

        var controller = GetComponent<PlayerController>();
        controller.SetDead(false);
        controller.enabled = true;

        var cc = GetComponent<CharacterController>();
        if (cc != null) { cc.enabled = false; transform.position = newPosition; cc.enabled = true; }

        var cam = controller.GetCameraTransform()?.gameObject;
        if (cam != null) cam.SetActive(true);
    }

    private void TeleportTo(Vector3 pos)
    {
        var cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        transform.position = pos;
        if (cc) cc.enabled = true;
    }

    public void ManualRespawn()
    {
        string scene = SceneManager.GetActiveScene().name;
        Transform spawnPoint = RespawnManager.Instance.GetSpawnPoint(scene);

        if (spawnPoint != null)
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            transform.position = spawnPoint.position;

            if (controller != null) controller.enabled = true;

            Debug.Log("[ManualRespawn] Jugador reposicionado.");
        }
        else
        {
            Debug.LogWarning("[ManualRespawn] No se encontró punto de respawn.");
        }
    }
}