using UnityEngine;
using Unity.Netcode;
using FinalCharacterController;

public class PlayerRespawn : NetworkBehaviour
{
    public Vector3 spawnPosition;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            spawnPosition = transform.position;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.position = spawnPosition;
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void OnRespawnClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetDead(false); 
            }
            var camera = controller.GetCameraTransform()?.gameObject;
            transform.position = spawnPosition;
            controller.enabled = true;
            if (camera != null) camera.SetActive(true);
        }
    }
}
