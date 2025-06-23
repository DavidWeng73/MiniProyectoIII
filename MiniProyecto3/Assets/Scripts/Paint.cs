using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Paint : NetworkBehaviour
{
    public static int numPaints = 0;
    public GameObject puerta;
    public GameObject player;

    public void DestroyPaint()
    {
        Debug.Log("Cuadro Completado");
        numPaints++;

        if (numPaints == 3 && puerta != null)
        {
            puerta.SetActive(false);
            HideDoorClientRpc();
        }

        HidePaintClientRpc();

        if (IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    private void HidePaintClientRpc()
    {
        if (!IsServer)
        {
            Debug.Log($"[Client] Ocultando cuadro con SetActive(false)");
            gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    private void HideDoorClientRpc()
    {
        if (!IsServer && puerta != null)
        {
            Debug.Log("[Client] Puerta desactivada con ClientRpc");
            puerta.SetActive(false);
        }
    }

    public void FakePaintTrap()
    {
        StartCoroutine(FakePaintTrapCoroutine());
    }

    private IEnumerator FakePaintTrapCoroutine()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            yield return new WaitForSeconds(5f);
            controller.enabled = true;
        }
    }
}
