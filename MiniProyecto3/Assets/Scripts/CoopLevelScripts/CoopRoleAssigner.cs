using UnityEngine;
using Unity.Netcode;

public class CoopRoleAssigner : NetworkBehaviour
{
    public enum CoopRole { Viewer, Shooter }

    public static CoopRole LocalRole;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            AssignRoleServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignRoleServerRpc(ulong clientId)
    {
        CoopRole role = (clientId % 2 == 0) ? CoopRole.Viewer : CoopRole.Shooter;
        AssignRoleClientRpc(clientId, role);
    }

    [ClientRpc]
    private void AssignRoleClientRpc(ulong clientId, CoopRole role)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            LocalRole = role;
            Debug.Log($"[CoopRoleAssigner] Rol asignado: {role}");

            if (role == CoopRole.Shooter)
                GameObject.Find("PaintsRoot").SetActive(false); 
        }
    }
}
