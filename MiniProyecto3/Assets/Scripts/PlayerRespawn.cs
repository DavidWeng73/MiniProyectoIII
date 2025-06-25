using UnityEngine;
using Unity.Netcode;
using FinalCharacterController;
using UnityEngine.SceneManagement;

public class PlayerRespawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoaded;
            TeleportToSpawn(SceneManager.GetActiveScene().name);
        }
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
        if (clientId != NetworkManager.LocalClientId) return;
        TeleportToSpawn(sceneName);
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
        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetDead(false);
            controller.enabled = true;
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = newPosition;

            if (cc != null) cc.enabled = true;

            var camera = controller.GetCameraTransform()?.gameObject;
            if (camera != null) camera.SetActive(true);
        }
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