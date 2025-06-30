using UnityEngine;
using System.Collections;
using Unity.Netcode;
using FinalCharacterController;

public class Paint : NetworkBehaviour
{
    public static int numPaints = 0;
    public GameObject puerta;

    void Start()
    {
        if (IsServer)
            numPaints = 0;
    }

    public void DestroyPaint()
    {
        numPaints++;
        if (numPaints == 3 && puerta != null)
            HideDoorClientRpc();

        HidePaintClientRpc();
        if (IsServer)
            gameObject.SetActive(false);
    }

    [ClientRpc]
    private void HidePaintClientRpc() => gameObject.SetActive(false);

    [ClientRpc]
    private void HideDoorClientRpc() => puerta.SetActive(false);

    // Nuevo: invoca la congelación en el cliente que disparó
    public void FakePaintTrap(ulong shooterClientId)
    {
        FreezePlayerClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { shooterClientId } }
        });
    }

    [ClientRpc]
    private void FreezePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var shooter = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ThirdPersonShooterController>();
        if (shooter != null)
        {
            shooter.TriggerFreezeFeedback(3f); // Duración de 3 segundos
        }
    }

    private IEnumerator FreezeCoroutine()
    {
        var controller = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            yield return new WaitForSeconds(5f);
            controller.enabled = true;
        }
    }
}
