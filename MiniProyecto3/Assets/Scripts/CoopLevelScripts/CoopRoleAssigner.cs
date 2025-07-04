using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CoopRoleAssigner : NetworkBehaviour
{
    public enum CoopRole { Viewer, Shooter }

    public static CoopRole LocalRole;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            // Cliente es Shooter
            AssignClientRoleClientRpc(NetworkManager.Singleton.LocalClientId, CoopRole.Shooter);
        }
        else if (IsOwner && IsHost)
        {
            // Host es Viewer
            AssignClientRoleClientRpc(NetworkManager.Singleton.LocalClientId, CoopRole.Viewer);
        }
    }

    [ClientRpc]
    private void AssignClientRoleClientRpc(ulong clientId, CoopRole role)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            LocalRole = role;
            Debug.Log($"[CoopRoleAssigner] Rol asignado: {role}");

            if (SceneManager.GetActiveScene().name == "CoopLevel1" && role == CoopRole.Shooter)
            {
                GameObject paintsRoot = GameObject.Find("PaintsRoot");
                if (paintsRoot != null)
                {
                    foreach (Renderer r in paintsRoot.GetComponentsInChildren<Renderer>())
                        r.enabled = false;
                }
            }
        }
    }
}
